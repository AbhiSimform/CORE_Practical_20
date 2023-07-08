﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practical_20.Interfaces;
using Practical_20.Models;

namespace Practical_20.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _repository;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<User>();
        }

        public async Task<IActionResult> Index()
        {
            return _repository.GetAll() != null ?
                        View(_repository.GetAll().ToList()) :
                        Problem("Entity set 'DatabaseContext.Users'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var user = _repository.GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _repository.Insert(user);
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var user = _repository.GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(user);
                    _unitOfWork.SaveChanges();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _repository.GetAll() == null)
            {
                return NotFound();
            }

            var user = _repository.GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_repository.GetAll() == null)
            {
                return Problem("Entity set 'DatabaseContext.Users'  is null.");
            }
            var user = _repository.GetById(id);
            if (user != null)
            {
                _repository.Delete(user);

            }

            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_repository.GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}