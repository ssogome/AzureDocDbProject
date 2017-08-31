using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureDocDbProject.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            var items = DocumentDBRepository.GetIncompleteItems();
            return this.View(items);
        }
    }
}