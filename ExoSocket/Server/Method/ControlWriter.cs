using System.Windows.Controls;

namespace Server
{
    /// <summary>
    /// 将TextBox变为Console
    /// </summary>
    public class ControlWriter: Core.Features.ControlWriter
    {
        public ControlWriter(TextBox textBox):base(textBox){ }
    }
}
