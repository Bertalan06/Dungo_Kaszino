using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace casino
{
    internal class Adatok
    {
        public string Nev { get; set; }
        public string Email { get; set; }
        public string FelhasznaloNev { get; set; }
        public string Telefonszam { get; set; }
        public string Jelszo { get; set; }
        public DateTime SzuletesiDatum { get; set; }
        public decimal Egyenleg { get; set; }
        public Adatok(string sor)
        {
            string[] adatok = sor.Split(';');
            Nev = adatok[0];
            Email = adatok[1];
            FelhasznaloNev = adatok[2];
            Telefonszam = adatok[3];
            Jelszo = adatok[4];
            try
            {
                SzuletesiDatum = DateTime.ParseExact(adatok[5], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                SzuletesiDatum = DateTime.Parse(adatok[5], new System.Globalization.CultureInfo("hu-HU"));
            }
            Egyenleg = 0; 
        }
    }
}
