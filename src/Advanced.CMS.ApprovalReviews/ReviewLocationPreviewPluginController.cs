using System.Linq;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Advanced.CMS.ApprovalReviews
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class ReviewLocationPreviewPluginController : Controller
    {
        private readonly IApprovalReviewsRepository _repository;

        public ReviewLocationPreviewPluginController(IApprovalReviewsRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var viewModel = new ViewModel
            {
                ControllerUrl = Paths.ToResource("advanced-cms-approval-reviews" ,"ReviewLocationPreviewPlugin")
            };

            return View("Index", viewModel);
        }

        public ActionResult GetAll()
        {
            var result = _repository.LoadAll().GroupBy(x => x.ContentLink.ToReferenceWithoutVersion()).Select(x => new
            {
                Id = x.Key,
                ContentLinks = x.Select(c => new { c.ContentLink, c.SerializedReview })
            });

            return new RestResult { Data = result };
        }

        [HttpPost]
        public void DeleteReviewLocation([FromBody] Dto dto)
        {
            _repository.Delete(ContentReference.Parse(dto.ContentLink));
        }
    }

    public class Dto
    {
        public string ContentLink { get; set; }
    }

    public class ViewModel
    {
        public string ControllerUrl { get; set; }
    }
}
