using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthProxyExample.Models;
using AuthProxyExample.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AuthProxyExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthorizedService _authorizedService;
        private readonly IIdentityService _identityService;

        public HomeController(ILogger<HomeController> logger,
            IAuthorizedService authorizedService, IIdentityService identityService)
        {
            _logger = logger;
            _authorizedService = authorizedService;
            _identityService = identityService;
        }

        public async Task<IActionResult> Login(string email, string fullname)
        {
            await _identityService.SignIn(email, fullname);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _identityService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index(bool isWithdraw = true)
        {
            if (isWithdraw)
                _authorizedService.WithdrawMoney(100);
            else
                _authorizedService.DepositMoney(100);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}