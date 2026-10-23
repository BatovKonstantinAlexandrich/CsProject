using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 
public class EmailOptions
{
    public string emailName;
    public string emailAddress;
    public string emailPassword;
    public string smtpHost;
    public int smtpPort;
    public string popHost;
    public int popPort;


    public EmailOptions()
    {
        this.emailName = "EcKuMoC";
        this.emailAddress = "kba-2021@mail.ru";
        this.emailPassword = "eckumoc1423";
        this.smtpHost = "smtp.mail.ru";
        this.smtpPort = 587;
        this.popHost = "pop.mail.ru";
        this.popPort = 995;
    }
}
 