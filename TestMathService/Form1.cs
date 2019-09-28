using System;
using System.Windows.Forms;
using TestMathService.MathServiceReference1;

namespace TestMathService
{
    using System.ServiceModel;

    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void ButtonAddClick(object sender, EventArgs e)
        {
            var client = new MathService1Client();
            var par1 = Int32.Parse(this.textBoxNum1.Text);
            var par2 = Int32.Parse(this.textBoxNum2.Text);
            this.labelResult.Text =$"{client.Add(par1, par2)}";

        }

        private void Button1Click(object sender, EventArgs e)
        {
            var client = new MathService1Client();
            var par1 = Int32.Parse(this.textBoxNum1.Text);
            var par2 = Int32.Parse(this.textBoxNum2.Text);
            try
            {
                this.labelResult.Text = $"{client.Divide(par1, par2)}";
            }
            catch (FaultException<FaultInfo> exception)
            {
                this.labelResult.Text = exception.Message;
            }
            catch (FaultException ex)
            {
                this.labelResult.Text = ex.Message;
            }
            catch (CommunicationException exception)
            {
                this.labelResult.Text = exception.Message;
            }
        }
    }
}
