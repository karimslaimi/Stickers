﻿using Model;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Stickers.Security;

namespace Stickers.Controllers
{
    public class ProductsController : Controller
    {

        serviceProduct sp = new serviceProduct();
        static bool order = false;
        // GET: Products
        public ActionResult Index()
        {
            IserviceProduct ip = new serviceProduct();
            List<Product> lp = new List<Product>();
            lp = ip.listprod();
            lp.Reverse();

            ViewBag.pagenb = lp.Count/12;
            return View(lp);
        }
        [HttpPost]
        public ActionResult Index(string search,string type,string btPage)
        {//this method maymeshaa haad w manajemch nfasarha li yheb yefhemha nfahemhelou ki net9ablou
            IserviceProduct ip = new serviceProduct();
            List<Product> lp=new List<Product>();
            if (type != null)
            {
                string ch = search;//valeur à chercher
                string ch1 = type;//type de trie
                lp= ip.search_kw(search).Where(a => a.qteprod > 0).ToList();
                if (!type.Equals("desc"))
                {
                    lp.Reverse();
                    order = true;
                }else
                { order = false; }
            }
            else
            { int page = Int32.Parse(btPage);

                lp = ip.listprod();
                if (order)
                {
                    lp.Reverse();
                  lp=lp.Skip((page - 1) * 12).ToList();
                }
                else
                {
                    lp = lp.Skip((page - 1) * 12).ToList();
                }
            }
            ViewBag.pagenb = ip.listprod().Count / 12;
            return View(lp);
        }


      

        // GET: Products/Details/5

        public ActionResult Details(int id,bool? stock,bool? error,bool? success)
        {
            IserviceProduct ip = new serviceProduct();
            Product p = ip.GetById(id);
            if (stock!=null && stock==true)
            {
                ViewBag.stock = "not enough quantity in the stock";
            }
            if (error!=null && error==true)
            {
                ViewBag.error = "be sure to check your informations all fields are required";
            }
            if (success!=null && success==true)
            {
                ViewBag.success = "your order is registred you will be contacted for delivery informations";

            }
            return View(p);
        }

        // GET: Products/Create
        [CustomAuthorizeAttribute(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }


        protected bool verifyFiles(HttpPostedFileBase item)
        {
            bool flag = true;
           
              
               
              if (item != null)
                    {
                if (item.ContentLength > 0 && item.ContentLength < 5000000)
                {


                    if (!(Path.GetExtension(item.FileName).ToLower() == ".jpg" ||
                        Path.GetExtension(item.FileName).ToLower() == ".png" ||
                        Path.GetExtension(item.FileName).ToLower() == ".bmp" ||
                        Path.GetExtension(item.FileName).ToLower() == ".jpeg"))
                    {
                        flag = false;
                    }





                }
                      else { flag = false; }

               }



                
            
                     else { flag = false; }

            return flag;
        }



        // POST: Products/Create
        [CustomAuthorizeAttribute(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Product prod, HttpPostedFileBase item, HttpPostedFileBase img1, HttpPostedFileBase img2, HttpPostedFileBase img3)
        {
            try
            {
                if (ModelState.IsValid && verifyFiles(item) && verifyFiles(img1) && verifyFiles(img2) && verifyFiles(img3))// check if the model state is valid , and the file (image in the input is valid)

                {
                    string name = "name" + prod.nameprod + "im" + DateTime.Now.Minute+DateTime.Now.Millisecond+ Path.GetExtension(item.FileName);
                  //creating the name of the product
                   var path = Path.Combine(Server.MapPath("../Content/stickerspic/"), name);
                    //creating the path of the image combining the name of the product with the minute and millisecond to get a unique name for it

                    item.SaveAs(path);
                    //image saved in the path
                    prod.imgprod = path;

                    prod.img1 = name + "one";
                    prod.img2 = name + "two";
                    prod.img3 = name + "three";
                    img1.SaveAs(Path.Combine(Server.MapPath("../Content/stickerspic/"), prod.img1));
                    img2.SaveAs(Path.Combine(Server.MapPath("../Content/stickerspic/"), prod.img2));
                    img3.SaveAs(Path.Combine(Server.MapPath("../Content/stickerspic/"), prod.img3));

                    sp.add_product(prod);
                }
                else
                {
                    ViewBag.error = "files or input are invalid";
                    return View();
                }

                return RedirectToAction("IndexProducts");
            }
            catch
            {
                return View();
            }
        }

    

   

        // GET: Products/Delete/5
        [CustomAuthorizeAttribute(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
           
            Product p = sp.GetById(id);
            Console.WriteLine("------------"+Path.Combine(Server.MapPath("/Content/stickerspic/"), p.imgprod));
            System.IO.File.Delete(Server.MapPath("/Content/stickerspic/")+ p.imgprod);
            Console.WriteLine(Path.Combine(Server.MapPath("/Content/stickerspic/"), p.imgprod));
            if (p.img1!=null && p.img1 != "")
            {
                System.IO.File.Delete(Server.MapPath("/Content/stickerspic/") + p.img1);
            }
            if (p.img2 != null && p.img2 != "")
            {
                System.IO.File.Delete(Server.MapPath("/Content/stickerspic/") + p.img2);
            }
            if (p.img3 != null && p.img3 != "")
            {
                System.IO.File.Delete(Server.MapPath("/Content/stickerspic/") + p.img3);
            }
            sp.deleteprod(id);
            return RedirectToAction("IndexProducts","Admin");
        }






        // POST: Products/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }







    }
}
