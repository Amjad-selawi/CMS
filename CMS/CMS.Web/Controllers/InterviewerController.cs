using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class InterviewerController : Controller
    {
        private readonly IInterviewerService _interviewerService;

        public InterviewerController(IInterviewerService interviewerService)
        {
            _interviewerService = interviewerService;
        }

        public async Task<IActionResult> Index()
        {
            var interviewers = await _interviewerService.GetAllInterviewersAsync();
            return View(interviewers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var interviewer = await _interviewerService.GetInterviewerByIdAsync(id);
            if (interviewer == null)
            {
                return NotFound();
            }
            return View(interviewer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InterviewerDTOs interviewerDTO)
        {
            if (ModelState.IsValid)
            {
                await _interviewerService.CreateInterviewerAsync(interviewerDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(interviewerDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var interviewer = await _interviewerService.GetInterviewerByIdAsync(id);
            if (interviewer == null)
            {
                return NotFound();
            }
            return View(interviewer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InterviewerDTOs interviewerDTO)
        {
            if (id != interviewerDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _interviewerService.UpdateInterviewerAsync(id, interviewerDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(interviewerDTO);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var interviewer = await _interviewerService.GetInterviewerByIdAsync(id);
            if (interviewer == null)
            {
                return NotFound();
            }
            return View(interviewer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _interviewerService.DeleteInterviewerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
