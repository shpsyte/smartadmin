using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{
    public interface IEntityTypeConfiguration<T> where T : class
    {
        void Map(EntityTypeBuilder<T> builder);
    }
}
