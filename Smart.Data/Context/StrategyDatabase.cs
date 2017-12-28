using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Context
{
    public static class DbInitializer
    {
        public static void Initialize(SmartContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
