using Newtonsoft.Json;
using ServerSide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ServerSide.Controllers
{
    
    public class ProcessController : ApiController
    {
        uPayEntities db = new uPayEntities();
        public HttpResponseMessage PostUser(User obj)
        {
            try
            {
                bool error = false;
                string errorMessage = "";

                if (obj.FullName == "")
                {
                    error = true;
                    errorMessage += "Please do not leave name area empty. ";
                }

                if (obj.FullName.Length < 4)
                {
                    error = true;
                    errorMessage += "Please check your name, it seems too short. ";
                }

                if (obj.Email.Contains('@') == false || obj.Email.Contains(".com") == false)
                {
                    error = true;
                    errorMessage += "Please check your e-mail. ";
                    
                }

                if (obj.UserName.Length < 3)
                {
                    error = true;
                    errorMessage += "Your username should contain at least 3 characters. ";
                }

                if (obj.Password.Length != 10)
                {
                    error = true;
                    errorMessage += "Your password should be consisting of 10 characters. ";
                }

                if(!error)
                {
                    db.Users.Add(obj);
                    db.SaveChanges();

                    var response = Request.CreateResponse(HttpStatusCode.Created, obj);
                    response.Headers.Location = new Uri(Request.RequestUri + obj.ID.ToString());
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);                    
                }                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Authorize]
        public HttpResponseMessage PostCard(AccountModel obj)
        {
            try
            {
                User user = db.Users.FirstOrDefault(x => x.UserName == obj.User.UserName && x.Password == obj.User.Password);
                Card card = new Card();

                bool error = false;
                string errorMessage = "";

                if (user.ID == 0)
                {
                    error = true;
                    errorMessage += "It seems like your session has been expired. Please sign in again. ";
                }
                else
                {
                    card.UserID = user.ID;
                }

                if (obj.Card.CardNumber == null || obj.Card.CardNumber.ToString() == "")
                {
                    error = true;
                    errorMessage += "Please fill the card number area. ";
                }
                else if (obj.Card.CardNumber.Length != 16)
                {
                    error = true;
                    errorMessage += "Card number should be consisting of 16 digits. ";
                }
                else
                {
                    card.CardNumber = obj.Card.CardNumber;
                }

                if (obj.Card.OwnerName == null || obj.Card.OwnerName.ToString() == "")
                {
                    error = true;
                    errorMessage += "Please fill the card owner area. ";
                }
                else if (obj.Card.OwnerName.Length < 4)
                {
                    error = true;
                    errorMessage += "Please check the owner name again, it seems too short. ";
                }
                else
                {
                    card.OwnerName = obj.Card.OwnerName;
                }

                int tryCVC = 0;                
                if (obj.Card.CVC == null || obj.Card.CVC.ToString() == "")
                {
                    error = true;
                    errorMessage += "Please fill the security number area. ";
                }
                else
                {
                    int.TryParse(obj.Card.CVC, out tryCVC);
                }

                if (tryCVC <= 0)
                {
                    error = true;
                    errorMessage += "Security number area should be consisting of only numbers. ";
                }

                if (obj.Card.CVC.Length != 3)
                {
                    error = true;
                    errorMessage += "Security number should be consisting of 3 digits. ";
                }
                else
                {
                    card.CVC = obj.Card.CVC;
                }

                if (obj.Card.Expiry == null || obj.Card.Expiry.ToString()  == "")
                {
                    error = true;
                    errorMessage += "Please fill the expiry date area. ";
                }
                else
                {
                    card.Expiry = obj.Card.Expiry;
                }

                if (!error)
                {
                    card.IsActive = obj.Card.IsActive;
                    db.Cards.Add(card);
                    db.SaveChanges();

                    var response = Request.CreateResponse(HttpStatusCode.Created, obj);
                    response.Headers.Location = new Uri(Request.RequestUri + obj.Card.ID.ToString());
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Authorize]
        public IHttpActionResult GetCard(string id)
        {
            User user = db.Users.FirstOrDefault(x => x.UserName == id);
            List<Card> card = db.Cards.Where(x => x.UserID == user.ID).ToList();
            return Json(card);
        }

        [Authorize]
        public IHttpActionResult GetDetails(int id)
        {
            Card card = db.Cards.FirstOrDefault(x => x.ID == id);
            return Json(card);
        }

        [Authorize]
        public HttpResponseMessage PutCard(int id, Card obj)
        {
            try
            {
                bool error = false;
                string errorMessage = "";

                Card card = db.Cards.FirstOrDefault(x => x.ID == id);

                if (card == null)
                {
                    error = true;
                    errorMessage += "Your account doesn't has such a payment method to update. Login again to select a current patment method. ";
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, errorMessage);
                }
                else
                {
                    if(obj.CardNumber == null || obj.CardNumber.ToString() == "")
                    {
                        error = true;
                        errorMessage += "Please fill the card number area. ";
                    }
                    else if (obj.CardNumber.Length != 16)
                    {
                        error = true;
                        errorMessage += "Card number should be consisting of 16 digits. ";
                    }
                    else
                    {
                        card.CardNumber = obj.CardNumber;
                    }

                    if (obj.OwnerName == null || obj.OwnerName.ToString() == "")
                    {
                        error = true;
                        errorMessage += "Please fill the card owner area. ";
                    }
                    else if (obj.OwnerName.Length < 4)
                    {
                        error = true;
                        errorMessage += "Please check the owner name again, it seems too short. ";
                    }
                    else
                    {
                        card.OwnerName = obj.OwnerName;
                    }

                    int tryCVC = 0;
                    if (obj.CVC == null || obj.CVC.ToString() == "")
                    {
                        error = true;
                        errorMessage += "Please fill the security number area. ";
                    }
                    else
                    {
                        int.TryParse(obj.CVC, out tryCVC);
                    }

                    if (tryCVC <= 0)
                    {
                        error = true;
                        errorMessage += "Security number area should be consisting of only numbers. ";
                    }

                    if (obj.CVC.Length != 3)
                    {
                        error = true;
                        errorMessage += "Security number should be consisting of 3 digits. ";
                    }
                    else
                    {
                        card.CVC = obj.CVC;
                    }

                    if (obj.Expiry == null || obj.Expiry.ToString() == "")
                    {
                        error = true;
                        errorMessage += "Please fill the expiry date area. ";
                    }
                    else
                    {
                        card.Expiry = obj.Expiry;
                    }

                    if (!error)
                    {
                        db.SaveChanges();
                        var response = Request.CreateResponse(HttpStatusCode.OK, obj);
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
                    }
                }   
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Authorize]
        public HttpResponseMessage PutActivation(int id)
        {
            try
            {
                Card card = db.Cards.FirstOrDefault(x => x.ID == id);
                if (card == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Your account doesn't has such a payment method to change. Login again to select a current patment method.");
                }
                else
                {
                    if (card.IsActive)
                    {
                        card.IsActive = false;
                    }
                    else
                    {
                        card.IsActive = true;
                    }
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }                
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }            
        }

        #region RedirectToAnotherPageFromApiController
        //[Authorize]
        //public HttpResponseMessage Get()
        //{
        //    var response = Request.CreateResponse(HttpStatusCode.Moved);
        //    response.Headers.Location = new Uri("http://localhost:51334/Menu.html");
        //    return response;
        //}
        #endregion

    }
}
