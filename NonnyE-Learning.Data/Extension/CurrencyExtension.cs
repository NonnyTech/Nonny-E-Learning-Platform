using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Extension
{
    public static class CurrencyExtension
    {
        public static int ToKobo(this decimal naira)
        {
            return (int)(naira * 100); // 1 Naira = 100 Kobo
        }
    }
}
