using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
  public interface IEmailManager 
  {
    IsEmail GetIsEmail(int emailId);

    void EnqueueEmail(int isEmailId, int delay);

    void CommitChanges();
  }
}
