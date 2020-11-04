﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms_SendMail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string[] toArr = { QQnumber.Text+ "@qq.com" };
            SendMail sendmail = new SendMail(toArr, "1659666966@qq.com", msgcontent.Text, subject.Text, "koqqjsdxeugnjeda", label4.Text);
            sendmail.Send();


            label1.Text = "发送成功";


        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
           

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                label4.Text = openFileDialog.FileName;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }


    public class SendMail
    {
        private MailMessage mailMessage;
        private SmtpClient smtpClient;
        private string password;//发件人密码 
        /// <summary> 
        /// 设置MailMessage的实例
        /// </summary> 
        /// <param name="To">收件人地址</param> 
        /// <param name="From">发件人地址</param> 
        /// <param name="Body">邮件正文</param> 
        /// <param name="Title">邮件的主题</param> 
        /// <param name="Password">发件人密码</param> 
        public SendMail(string[] To, string From, string Body, string Title, string Password,string filefath)
        {
            mailMessage = new MailMessage();
            foreach (var item in To)
            {
                mailMessage.To.Add(item);
            }
            mailMessage.From = new System.Net.Mail.MailAddress(From);
            mailMessage.Subject = Title;
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
            this.password = Password;

            if (filefath.Length > 0)
            {
                Attachments(filefath);
            }
        }
        /// <summary> 
        /// 添加附件 
        /// </summary> 
        public void Attachments(string Path)
        {
            string[] path = Path.Split(',');
            Attachment data;
            ContentDisposition disposition;
            for (int i = 0; i < path.Length; i++)
            {
                data = new Attachment(path[i], MediaTypeNames.Application.Octet);//实例化附件 
                disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(path[i]);//获取附件的创建日期 
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path[i]);//获取附件的修改日期 
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path[i]);//获取附件的读取日期 
                mailMessage.Attachments.Add(data);//添加到附件中 
            }
        }
        /// <summary> 
        /// 异步发送邮件 
        /// </summary> 
        /// <param name="CompletedMethod"></param> 
        public void SendAsync(SendCompletedEventHandler CompletedMethod)
        {
            if (mailMessage != null)
            {
                smtpClient = new SmtpClient();
                smtpClient.EnableSsl = true;// 指定 System.Net.Mail.SmtpClient 是否使用安全套接字层 (SSL) 加密连接,必须在实例身份前面设置
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据 
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.Host = "smtp." + mailMessage.From.Host;
                smtpClient.Port = 587;
                smtpClient.SendCompleted += new SendCompletedEventHandler(CompletedMethod);//注册异步发送邮件完成时的事件 
                smtpClient.SendAsync(mailMessage, mailMessage.Body);
            }
        }
        /// <summary> 
        /// 发送邮件 
        /// </summary> 
        public void Send()
        {
            if (mailMessage != null)
            {
                using (smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = true;// 指定 System.Net.Mail.SmtpClient 是否使用安全套接字层 (SSL) 加密连接,必须在实例身份前面设置
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据           
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp." + mailMessage.From.Host;
                    smtpClient.Send(mailMessage);
                    mailMessage.Attachments.Dispose();
                }
            }
        }
    }
}
