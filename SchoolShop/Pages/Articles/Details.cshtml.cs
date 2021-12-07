using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolShop.Data;
using SchoolShop.Models;
using SchoolShop.Helpers;
namespace SchoolShop.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly SchoolShop.Data.SchoolShopContext _context;
        public Availability Availability { get; set; }
        public int? PreviousId { get; set; }
        public int? NextId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        private List<Article> Articles { get; set; }
        [BindProperty]
        public Article Article { get; set; }

        public DetailsModel(SchoolShop.Data.SchoolShopContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int? id, int? categoryid, int? brandid)
        {
            Availability = new Availability(_context, HttpContext);

            if (id == null)
            {
                return NotFound();
            }

            Article = _context.Article
                .Include(a => a.Brand)
                .Include(a => a.Category).FirstOrDefault(m => m.Id == id);

            if (Article == null)
            {
                return NotFound();
            }

            // vraag alle artikels op
            // sorteer ze op dezelfde manier als de index pagina
            // filter ze op dezelfde manier als de index pagina
            IQueryable<Article> artquery = _context.Article
                            .OrderBy(a => a.Category.CategoryName)
                            .ThenBy(a => a.Price);
            if (brandid != null && categoryid == null)
            {
                artquery = artquery.Where(b => b.BrandId.Equals(brandid));
            }
            if (brandid == null && categoryid != null)
            {
                artquery = artquery.Where(b => b.CategoryId.Equals(categoryid));
            }
            if (brandid != null && categoryid != null)
            {
                artquery = artquery.Where(b => b.BrandId.Equals(brandid) && b.CategoryId.Equals(categoryid));
            }
            Articles = artquery.ToList();
            BrandId = brandid;
            CategoryId = categoryid;

            PreviousId = null;
            NextId = null;
            // ga op zoek naar het id van vorige en volgende
            for (int i = 0; i < Articles.Count; i++)
            {
                if (((Article)Articles[i]).Id == id)
                {
                    if (i > 0)
                        PreviousId = ((Article)Articles[i - 1]).Id;
                    if (i < Articles.Count - 1)
                        NextId = ((Article)Articles[i + 1]).Id;
                    break;
                }
            }
            if (PreviousId == null) PreviousId = id;
            if (NextId == null) NextId = id;

            return Page();
        }
    }
}
