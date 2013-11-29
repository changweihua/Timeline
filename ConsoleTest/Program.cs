using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace ConsoleTest
{
    class Program
    {
       
        static void Main(string[] args)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("student.xml");
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("姓名", typeof(string)));
            dt.Columns.Add(new DataColumn("性别", typeof(int)));
            dt.Columns.Add(new DataColumn("电话号码", typeof(string)));
            dt.Columns.Add(new DataColumn("公司", typeof(string)));
            XmlNode node = xmlDoc.SelectSingleNode("/look/book[phone = 2222222]");
            Console.WriteLine(node.ChildNodes.Count);
            string colName;
            if (node != null)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    colName = node.ChildNodes.Item(i).Name;
                    dt.Columns.Add(colName);
                    
                }
            }

            DataRow row = dt.NewRow();
            row[0] = node.Attributes["name"].Value;
            row[1] = node.ChildNodes.Item(0).InnerText;
            row[2] = node.ChildNodes.Item(1).InnerText;
            dt.Rows.Add(row);
            Console.WriteLine("{0}\t{1}\t{2}", row[0], row[1], row[2]);
            //DataSet ds = new DataSet("book");
            //ds.ReadXml("student.xml");
            //ds.Tables.Add(dt);
            ////datagridview1.
            //Console.WriteLine(ds.Tables[0].Rows.Count);

            Console.ReadKey(true);
        }

        static void Calc(int count)
        { 
            
        }

    }
}
