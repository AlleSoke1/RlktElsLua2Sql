using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RlktElsLuaToSql
{
    public class RlktLogger
    {
        public enum LogType
        {
            FILE,
            FORM_RICHTEXT,
        }

        //
        static LogType m_LogType;
        static RichTextBox m_outRichTextBox = null;
        static List<string> m_LogText = new List<string>();

        //

        public static void SetLogType(LogType logType)
        {
            m_LogType = logType;
        }

        public static void SetLogOutput(RichTextBox richTextBox)
        {
            m_outRichTextBox = richTextBox;
        }

        public static void Log(string message, Color color = default)
        {
            m_LogText.Add(message);

            switch (m_LogType)
            {
                case LogType.FORM_RICHTEXT: LogToRichText(message, color); break;
                case LogType.FILE:          LogToFile(message, color); break;
            }
        }

        static void LogToRichText(string message, Color color)
        {
            if (m_outRichTextBox == null)
                return;

            m_outRichTextBox.SelectionStart = m_outRichTextBox.TextLength;
            m_outRichTextBox.SelectionLength = 0;

            m_outRichTextBox.SelectionColor = color;
            m_outRichTextBox.AppendText(message + Environment.NewLine);
            m_outRichTextBox.SelectionColor = Color.Black;
        }

        static void LogToFile(string message, Color color)
        {
            //todo
        }
    }
}
