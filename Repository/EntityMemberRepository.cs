using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using VideoLoaderMVC.Repository.Interfaces;
using VideoLoaderMVC.Models;

namespace VideoLoaderMVC.Repository
{
    public class EntityMemberRepository : IMemberRepository
    {
        public int Create(RegisterModel member)
        {
            int recordID = 0;
            string[] nameSections;

            using (ksalomon_listEntities db = new ksalomon_listEntities())
            {
                nameSections = Regex.Split(member.UserName, @"\s+");
                // Create a new Order object.
                Member ord = new Member
                {
                    FirstName = nameSections.Count() > 1 ? nameSections[0].Trim() : string.Empty,
                    LastName = nameSections.Count() > 1 ? nameSections[1].Trim() : string.Empty,                        
                    Email = member.Email,
                    Password = member.Password,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };

                // Add the new object to the Members collection.
                db.Members.Add(ord);

                // Save the change to the database.
                try
                {
                    db.SaveChanges();
                    recordID = ord.id; //return this
                }
                catch 
                {
                    return 0;
                }

                return recordID;
            }
        }

        public bool Update(MemberModel member)
        {
            return true;
        }

        public bool Delete(int id)
        {
            return true;
        }

        public MemberModel Get(int id)
        {
            return null;
        }

        public List<MemberModel> GetAll()
        {
            return null;
        }

        public int ValidateMember(string userName, string passWord)
        {
            try
            {
                using (ksalomon_listEntities db = new ksalomon_listEntities())
                {
                    Member member = (from m in db.Members where m.Email == userName.Trim() && m.Password == passWord.Trim() select m).FirstOrDefault();
                    if (member != null)
                    {
                        return member.id; //Return UserID
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

    }
}