using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoLoaderMVC.Models;

namespace VideoLoaderMVC.Repository.Interfaces
{
    public interface IMemberRepository
    {
        int Create(RegisterModel member);
        bool Update(MemberModel member);
        bool Delete(int id);
        MemberModel Get(int id);
        List<MemberModel> GetAll();
        int ValidateMember(string userName, string passWord);
    }
}