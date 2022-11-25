using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class Datalogger
    {
        string currentFileName = createFileName();
        char delimeter = ';';
        public static string folder = Val.PathTemperatures;
        StreamWriter sw;
        bool isActive = false;
        public Datalogger()
        {            
            checkPath();
        }

        void checkPath()
        {
            if (!Directory.Exists(Val.PathTemperatures))
            {
                MessageBox.Show("Path " + folder + " doesnt exists. Check advanced settings for temperatures log path entry and provide valid folder path.");
                Environment.Exit(0);
            }         
            
        }
       
        static string createFileName()
        {
            var datetime =
                DateTime.Now.Year.ToString() + " " + DateTime.Now.Month.ToString() + " " + DateTime.Now.Day.ToString() + " " +
                DateTime.Now.Hour.ToString() + " " + DateTime.Now.Minute.ToString() + " " + DateTime.Now.Second.ToString();
            string buff = folder + "\\" + datetime + ".csv";
            return buff;
        }

        void createHeader()
        {
            var col0 = "Time";           
            var col1 = "Sensor1";
            var col2 = "Sensor2";
            var col3 = "Sensor3";
            var col4 = "Sensor4";
            var col5 = "SensorKanal";
            var col6 = "SensorKos";

            var concat = col0 + delimeter + col1 + delimeter + col2 + delimeter + col3 + delimeter + col4 + delimeter + col5 + delimeter + col6;
            sw = new StreamWriter(currentFileName, true);
            sw.WriteLine(concat);
            sw.Close();
            sw.Dispose();
        }

        public void StartCsvFileWriter()
        {
            isActive = true;           
            createHeader();
        }

        public void StopCsvFileWriter()
        {
            isActive = false;
            sw.Dispose();
            sw = null;
        }

        public void WriteLine(ChartDataPoint Sensor1, ChartDataPoint Sensor2, ChartDataPoint Sensor3, ChartDataPoint Sensor4, ChartDataPoint SensorKanal, ChartDataPoint SensorKos)
        {
            isActive = true;

            var col0 = Sensor1.Time.ToString();
            var col1 = Sensor1.Value;
            var col2 = Sensor2.Value;
            var col3 = Sensor3.Value;
            var col4 = Sensor4.Value;            
            var col5 = SensorKanal.Value;
            var col6 = SensorKos.Value;

            var concat = col0 + delimeter + col1 + delimeter + col2 + delimeter + col3 + delimeter + col4 + delimeter + col5 + delimeter + col6;

            
            sw = new StreamWriter(currentFileName, true);
            sw.AutoFlush = true;
            sw.WriteLine(concat);
            sw.Close();
            sw.Dispose();
        }

    }
}
