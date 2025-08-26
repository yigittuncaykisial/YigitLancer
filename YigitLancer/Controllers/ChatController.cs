using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Security.Claims;
using Entities.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chat;
        private readonly IUserService _users;

        public ChatController(IChatService chat, IUserService users)
        {
            _chat = chat;
            _users = users;
        }

        private int? CurrentUserId()
        {
            var s = User.FindFirstValue("UserId");
            return int.TryParse(s, out var id) ? id : (int?)null;
        }

        public IActionResult Index()
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var list = _chat.GetConversationsForUser(uid.Value);
            var vm = list.Select(c =>
            {
                var me = uid.Value;
                var other = (c.BuyerUserId == me) ? c.Freelancer : c.Buyer;
                return new ConversationListItemVM
                {
                    ConversationId = c.Id,
                    JobName = c.Job?.JobName ?? "(İş Yok)",
                    OtherUserName = other?.UserName ?? "—",
                    OtherUserAvatar = other?.ProfileImagePath ?? "/uploads/default.png",
                    CreatedAt = c.CreatedAt
                };
            }).ToList();

            return View(vm);
        }

        public IActionResult Conversation(int id)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var conv = _chat.GetConversation(id, uid.Value);
            if (conv == null) return NotFound();

            var msgs = _chat.GetMessages(id);
            var other = conv.BuyerUserId == uid.Value ? conv.Freelancer : conv.Buyer;

            var vm = new ConversationDetailVM
            {
                ConversationId = conv.Id,
                Job = conv.Job,
                Other = other,
                Messages = msgs
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(int id, string text)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Index", "Auth");

            var conv = _chat.GetConversation(id, uid.Value);
            if (conv == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(text))
                _chat.SendMessage(id, uid.Value, text);

            return RedirectToAction("Conversation", new { id });
        }

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

            // Rol ayrımı: mevcut kullanıcı Buyer ise karşı taraf Freelancer, değilse tersi
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
    }

    public class ConversationListItemVM
    {
        public int ConversationId { get; set; }
        public string JobName { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ConversationDetailVM
    {
        public int ConversationId { get; set; }
        public Jobs? Job { get; set; }
        public User? Other { get; set; }
        public List<Message> Messages { get; set; } = new();
    }

    public class ChatCreateVM
    {
        public List<SelectListItem> Users { get; set; } = new();
        public int SelectedUserId { get; set; }
        public int? JobId { get; set; }
    }
}
