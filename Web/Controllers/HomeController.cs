using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork<Owner> _owner;
        private readonly IUnitOfWork<PortofolioItem> _portofolioItem;

        public HomeController(IUnitOfWork<Owner> owner, IUnitOfWork<PortofolioItem> PortofolioItem)
        {
            _owner = owner;
            _portofolioItem = PortofolioItem;
        }
        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                Owner = _owner.Entity.GetAll().First(),
                PortofolioItems = _portofolioItem.Entity.GetAll().ToList()
            };
            return View(homeViewModel);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
