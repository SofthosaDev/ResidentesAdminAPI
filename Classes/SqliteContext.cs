using System.Collections.Generic;
using WsAdminResidentes.Models.Utilidades;
using Microsoft.EntityFrameworkCore;
using WsAdminResidentes.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace WsAdminResidentes.Services.Utilidades
{
    public class SqliteContext : DbContext
    {
        private readonly AppSettings _appSettings;
        public DbSet<ArchivoTemporal> ArchivosTemporales { get; set; }
        public SqliteContext(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value as AppSettings;
            this.Database.EnsureCreated();
            //this.Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            DbContextOptionsBuilder opt = options;
            if (this._appSettings.RutaArchivos == null || this._appSettings.RutaArchivos.ToString().Trim() == "")
            {
                throw (new Exception("No se ha definido la ruta donde se guardan los archivos"));
            }
            string db_ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._appSettings.SQLITE_DB);
            options.UseSqlite($"Data Source={db_ruta}");
        }
    }
}
