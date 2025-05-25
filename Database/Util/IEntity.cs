using Microsoft.EntityFrameworkCore;

namespace Database.Util;

public interface IEntity
{
    static abstract void Configure(ModelBuilder modelBuilder);
}