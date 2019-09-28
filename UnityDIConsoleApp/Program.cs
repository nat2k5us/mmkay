using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityDIConsoleApp
{
    using Microsoft.Practices.Unity;

    public interface IEmployee
    {
    }
    public class Employee : IEmployee
    {
        private ICompany company;
        [Dependency]
        public ICompany Company
        {
            get { return this.company; }
            set { this.company = value; }
        }
        public void DisplaySalary()
        {
            this.company.ShowSalary();
        }
    }

    public interface ICompany
    {
        void ShowSalary();
    }
    public class Company : ICompany
    {
        public void ShowSalary()
        {
            Console.WriteLine("Your salary is 40 K");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer unitycontainer = new UnityContainer();
            unitycontainer.RegisterType<ICompany, Company>();

            Employee emp = unitycontainer.Resolve<Employee>();
            emp.DisplaySalary();


            Console.ReadLine();
        }
    }
}
