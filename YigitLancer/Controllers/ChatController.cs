using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Services.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using YigitLancer.Hubs;

namespace Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chat;
        private readonly IUserService _users;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(IChatService chat, IUserService users, IHubContext<ChatHub> hub)
        {
            _chat = chat;
            _users = users;
            _hub = hub;
        }

        private int? CurrentUserId()
        {
            var s = User.FindFirstValue("UserId");
            return int.TryParse(s, out var id) ? id : (int?)null;
        }

        // SOHBET LİSTESİ
        public IActionResult Index()
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            // Not: GetConversationsForUser içinde sıralamayı "son mesaj zamanı"na göre yaptığını varsayıyorum.
            // (ChatManager'da OrderByDescending(Max(Message.CreatedAt) ?? Conversation.CreatedAt) olmalı.)
            var list = _chat.GetConversationsForUser(uid.Value);

            var vm = list.Select(c =>
            {
                var me = uid.Value;
                var other = (c.BuyerUserId == me) ? c.Freelancer : c.Buyer;
                var unread = _chat.GetUnreadCountForConversation(c.Id, me);

                return new ConversationListItemVM
                {
                    ConversationId = c.Id,
                    JobName = c.Job?.JobName ,
                    OtherUserName = other?.UserName ?? "—",
                    OtherUserAvatar = other?.ProfileImagePath ?? "/uploads/default.png",
                    CreatedAt = c.CreatedAt,
                    UnreadCount = unread
                };
            }).ToList();

            return View(vm);
        }

        // TEK SOHBET SAYFASI
        public async Task<IActionResult> Conversation(int id)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var conv = _chat.GetConversation(id, uid.Value);
            if (conv == null) return NotFound();

            // Benim olmayan okunmamışları okundu yap
            _chat.MarkMessagesRead(id, uid.Value);

            // Rozeti iki tarafta güncelle
            var otherId = (conv.BuyerUserId == uid.Value) ? conv.FreelancerUserId : conv.BuyerUserId;
            await _hub.Clients.Group($"user-{uid.Value}").SendAsync("UnreadChanged");
            await _hub.Clients.Group($"user-{otherId}").SendAsync("UnreadChanged");

            // (İstersen) konuşma grubuna da "okundu" olayı
            await _hub.Clients.Group($"conv-{id}").SendAsync("MessagesRead", new { conversationId = id });

            var msgs = _chat.GetMessages(id);
            var other = conv.BuyerUserId == uid.Value ? conv.Freelancer : conv.Buyer;

            var vm = new ConversationDetailVM
            {
                ConversationId = conv.Id,
                Job = conv.Job,
                Other = other,
                Messages = msgs,
                // yalnız son mesaj için değil; View'da her benim mesajım için m.IsRead kullanacağız.
                // Bu property yine de lazım olabilir diye bırakıyorum:
                LastMessageRead = _chat.IsLastMessageRead(conv.Id, uid.Value)
            };
            return View(vm);
        }

        // MESAJ GÖNDER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int id, string text)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var conv = _chat.GetConversation(id, uid.Value);
            if (conv == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var msg = _chat.SendMessage(id, uid.Value, text);

                // Karşı tarafa unread güncelle
                var recipientId = (conv.BuyerUserId == uid.Value) ? conv.FreelancerUserId : conv.BuyerUserId;
                await _hub.Clients.Group($"user-{recipientId}").SendAsync("UnreadChanged");

                // Konuşma açık olanlara canlı mesaj
                await _hub.Clients.Group($"conv-{id}").SendAsync("MessageCreated", new
                {
                    conversationId = id,
                    senderUserId = uid.Value,
                    text = msg.Text,
                    createdAt = msg.CreatedAt
                });
            }

            return RedirectToAction("Conversation", new { id });
        }

        // YENİ SOHBET OLUŞTUR (GET)
        [HttpGet]
        public IActionResult Create(int? jobId)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var users = _chat.GetAllUsersExcept(uid.Value);
            var items = users.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = $"{u.UserName} ({u.UserJob})"
            }).ToList();

            var vm = new ChatCreateVM
            {
                Users = items,
                JobId = jobId
            };
            return View(vm);
        }

        // YENİ SOHBET OLUŞTUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ChatCreateVM vm)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            if (vm.SelectedUserId <= 0)
            {
                ModelState.AddModelError("", "Lütfen bir kullanıcı seçin.");
                return View(vm);
            }
            if (vm.SelectedUserId == uid.Value)
            {
                ModelState.AddModelError("", "Kendinizle sohbet başlatamazsınız.");
                return View(vm);
            }

            var me = _users.GetUserById(uid.Value);
            var other = _users.GetUserById(vm.SelectedUserId);
            if (me == null || other == null) return NotFound();

            int buyerId, freelancerId;
            if ((me.UserJob ?? "").Equals("Buyer", StringComparison.OrdinalIgnoreCase))
            {
                buyerId = me.UserId;
                freelancerId = other.UserId;
            }
            else
            {
                buyerId = other.UserId;
                freelancerId = me.UserId;
            }

            var conv = _chat.GetOrCreateConversation(buyerId, freelancerId, vm.JobId);
            return RedirectToAction("Conversation", new { id = conv.Id });
        }

        // NAVBAR ROZETİ (AJAX)
        [HttpGet]
        public IActionResult UnreadSummary()
        {
            var uid = CurrentUserId();
            if (uid == null) return Unauthorized();

            var convCount = _chat.GetUnreadConversationCount(uid.Value); // istersen kullan
            var totalCount = _chat.GetTotalUnreadCount(uid.Value);       // NAVBAR için bunu kullanıyoruz (B seçimi)

            return Json(new { conversations = convCount, messages = totalCount });
        }
    }

    // ==== VIEW MODELLER ====

    public class ConversationListItemVM
    {
        public int ConversationId { get; set; }
        public string JobName { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UnreadCount { get; set; }   // liste satırı için rozet
    }

    public class ConversationDetailVM
    {
        public int ConversationId { get; set; }
        public Jobs? Job { get; set; }
        public User? Other { get; set; }
        public List<Message> Messages { get; set; } = new();
        public bool LastMessageRead { get; set; }
    }

    public class ChatCreateVM
    {
        public List<SelectListItem> Users { get; set; } = new();
        public int SelectedUserId { get; set; }
        public int? JobId { get; set; }
    }
}
