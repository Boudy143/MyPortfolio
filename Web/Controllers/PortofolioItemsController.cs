using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Infrastructure;
using Web.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Core.Interfaces;

namespace Web.Controllers
{
    public class PortofolioItemsController : Controller
    {
        private readonly IUnitOfWork<PortofolioItem> _portfolio;
        private readonly IHostingEnvironment _hosting;

        public PortofolioItemsController(IUnitOfWork<PortofolioItem> portfolio, IHostingEnvironment hosting)
        {
            _portfolio = portfolio;
            _hosting = hosting;
        }

        // GET: PortofolioItems
        public IActionResult Index()
        {
            return View(_portfolio.Entity.GetAll());
        }

        // GET: PortofolioItems/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portofolioItem = _portfolio.Entity.GetById(id);

            if (portofolioItem == null)
            {
                return NotFound();
            }

            return View(portofolioItem);
        }

        // GET: PortofolioItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PortofolioItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PortfolioViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.File != null)
                {
                    string uploads = Path.Combine(_hosting.WebRootPath, @"img\portfolio");
                    string fullPath = Path.Combine(uploads, model.File.FileName);
                    model.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
                PortofolioItem portofolioItem = new PortofolioItem
                {
                    ProjectName = model.ProjectName,
                    Description = model.Description,
                    ImageUrl = model.File.FileName
                };
                _portfolio.Entity.Insert(portofolioItem);
                _portfolio.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: PortofolioItems/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portofolioItem = _portfolio.Entity.GetById(id);
            if (portofolioItem == null)
            {
                return NotFound();
            }
            PortfolioViewModel portfolioViewModel = new PortfolioViewModel
            {
                Id = portofolioItem.Id,
                Description = portofolioItem.Description,
                ImageUrl = portofolioItem.ImageUrl,
                ProjectName = portofolioItem.ProjectName
            };
            return View(portfolioViewModel);
        }

        // POST: PortofolioItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, PortfolioViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.File != null)
                    {
                        string uploads = Path.Combine(_hosting.WebRootPath, @"img\portfolio");
                        string fullPath = Path.Combine(uploads, model.File.FileName);
                        model.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                    }
                    PortofolioItem portofolioItem = new PortofolioItem
                    {
                        Id = model.Id,
                        ProjectName = model.ProjectName,
                        Description = model.Description,
                        ImageUrl = model.File.FileName
                    };
                    _portfolio.Entity.Update(portofolioItem);
                    _portfolio.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortofolioItemExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: PortofolioItems/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portofolioItem = _portfolio.Entity.GetById(id);
            if (portofolioItem == null)
            {
                return NotFound();
            }

            return View(portofolioItem);
        }

        // POST: PortofolioItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _portfolio.Entity.Delete(id);
            _portfolio.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool PortofolioItemExists(Guid id)
        {
            return _portfolio.Entity.GetAll().Any(e => e.Id == id);
        }
    }
}
