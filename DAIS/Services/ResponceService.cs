using DAIS.Helpers;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DAIS.Dialogs.BuiltIn;

namespace DAIS.Services
{
    public sealed class ResponseService : IResponseService
    {
        public string CreateGreetingsResponce(LuisResult request)
        {
            return "Hi my name is DAIS - Diatom's artificial intelligence system. Let me help you!";
        }

        public string CreateNoneResponse(LuisResult request)
        {
            var entity = request.Entities.FirstOrDefault()?.Entity;
            if (string.IsNullOrEmpty(entity))
            {
                entity = request.Query;
            }

            if (entity.CompareAndEquals("thanks", "thank you", "thank you dais", "thanks dais"))
            {
                return "Your welcome";
            }

            if (entity.CompareAndEquals("this is very good", "this is good"))
            {
                return "Thank you";
            }
            if (entity.CompareAndEquals("dias", "dais"))
            {
                return "That is how Alex called me";
            }
            if (entity.CompareAndEquals("languages"))
            {
                return "Only English for now";
            }
            if (entity.CompareAndEquals("where is riga"))
            {
                return "Try to find on the map";
            }
            if (entity.CompareAndEquals("weather"))
            {
                return "Im not a stupid weather bot!";
            }

            if (entity.CompareAndEquals("aleksandr litvinenko"))
            {
                return "Hi is my father";
            }


            if (entity.CompareAndEquals("dasha"))
            {
                return "Please, tell me";
            }

            else
            {
                return "Sorry I dont undersatand";
            }
        }

        public string CreateCompanyInfoResponse(LuisResult request)
        {
            var informationTypeEntity = request.Entities.FirstOrDefault(e => e.Type == Entities.InformationType);
            var LangugeEntity = request.Entities.FirstOrDefault(e => e.Entity == Entities.Language);
            var technologyEntity = request.Entities.FirstOrDefault(e => e.Type == Entities.Technology);

            if (informationTypeEntity != null)
            {
                var entity = informationTypeEntity.Entity;

                if (entity.CompareAndEquals("how many developers"))
                {
                    return "71";
                }

                else if (entity.CompareAndEquals("call me", "phone me"))
                {
                    return "Please, give me your phone number and name";
                }

                else if (entity.CompareAndEquals("founders of diatom"))
                {
                    return "Denis Gorshkov is Diatoms founder";
                }

                else if (entity.CompareAndEquals("specialty", "technology", "technologies", "technology diatom", "company profile"))
                {
                    return ".net, ror, angularjs, nodejs etc...";
                }

                else if (entity.CompareAndEquals("what do you know?"))
                {
                    return "More than you! ;)";
                }

                else if (entity.CompareAndEquals("name of your company"))
                {
                    return "It's name Diatom!";
                }

                else if (entity.CompareAndEquals("call you"))
                {
                    return "Our phone number is 222.22.22.222";
                }

                else if (entity.CompareAndEquals("about your projects"))
                {
                    return "We have a lot of projects";
                }

                else if (entity.CompareAndEquals("located", "address", "you from", "are you from", "diatom from"))
                {
                    return "We are in Riga Straupes 5, k-1,";
                }

                else if (entity.CompareAndEquals("about company",
                    "about your company",
                    "diatom",
                    "information about diatom",
                    "something about diatom"
                    ))
                {
                    return "Diatom was founded in 1991 ....";
                }
                else if (entity.CompareAndEquals("create"))
                {
                    if (request.Entities.Any(e => e.Type == Entities.Technology))
                    {
                        return "Yes we can!";
                    }

                    if (request.Entities.Any(e => e.Type == Entities.Language))
                    {
                        return string.Format("Yes we can. We have experts in {0}", request.Entities.First(e => e.Type == Entities.Language).Entity);
                    }
                }

                else if (entity.CompareAndEquals("contact you", "email", "phone number"))
                {
                    return "Our email is diatom@rer.er anf our phone is 222.222.22";
                }

                else if (entity.CompareAndEquals("contact you"))
                {
                    return "Our email is diatom@rer.er anf our phone is 222.222.22";
                }

                else if (entity.CompareAndEquals("experts"))
                {
                    if (technologyEntity != null)
                    {
                        return string.Format("Yes we have experts in {0}", technologyEntity.Entity);
                    }
                    if (LangugeEntity != null)
                    {
                        return string.Format("Yes we have experts in {0}", LangugeEntity.Entity);
                    }
                    return "Yes we have an experts in this technology";
                }

                else if (entity.CompareAndEquals("create"))
                {
                    if (technologyEntity != null)
                    {
                        return string.Format("Yes we can create {0}", technologyEntity.Entity);
                    }
                    return "Yes we have an experts in this technology";
                }

                else if (entity.CompareAndEquals("call you"))
                {
                    return "Our phone is 222.222.22";
                }

                else if (entity.CompareAndEquals("ceo", "head of company"))
                {
                    return "Our SEO is Denis Gorshkov";
                }

                else if (entity.CompareAndEquals("cto", "head of technology"))
                {
                    return "Our CTO is Vyacheslav Dubovitsky";
                }
                else if (entity.CompareAndEquals("coo", "head of operations"))
                {
                    return "Our COO is Evgueny Lemasov";
                }

            }
            else if (technologyEntity != null)
            {
                var entity = technologyEntity.Entity;
                return "Yes we have experts in " + entity;
            }
            else if (LangugeEntity != null)
            {

            }

            return "sorry I did not have an answer on this so far. But I learn fast.";
        }

        public string TryGetEmail(LuisResult result)
        {
            var entity = result.Entities.FirstOrDefault()?.Entity;
            if(entity.CompareAndEquals("Slava", "Slava Dubovitsky", "Dubovitsky", "CTO"))
            {
                return "slavadubovitsky";
            }
            else if(entity.CompareAndEquals("alex"))
            {
                return "alexemail";
            }
            return null;
        }
    }
}