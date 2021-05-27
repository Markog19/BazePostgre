using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApplication6.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumKorisnik> AlbumKorisniks { get; set; }
        public virtual DbSet<Band> Bands { get; set; }
        public virtual DbSet<Glazbenik> Glazbeniks { get; set; }
        public virtual DbSet<IzdavačkaKuća> IzdavačkaKućas { get; set; }
        public virtual DbSet<Korisnik> Korisniks { get; set; }
        public virtual DbSet<MjestoRođenja> MjestoRođenjas { get; set; }
        public virtual DbSet<Pjesma> Pjesmas { get; set; }
        public virtual DbSet<PjesmaKorisnik> PjesmaKorisniks { get; set; }
        public virtual DbSet<ČlanoviBendum> ČlanoviBenda { get; set; }
        public virtual DbSet<Žanr> Žanrs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=localhost;Database=glazbeni_studio;User Id=postgres;Password=markog1987;Port=5432");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Croatian_Croatia.1252");

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");

                entity.HasIndex(e => e.IzvođačId, "fki_album_to_band_id");

                entity.HasIndex(e => e.IzdavačkaKućaId, "fki_album_to_izdavacka_kuca");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GodinaIzdavanja)
                    .HasColumnType("date")
                    .HasColumnName("godina_izdavanja");

                entity.Property(e => e.IzdavačkaKućaId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("izdavačka_kuća_id");

                entity.Property(e => e.IzvođačId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("izvođač_id");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("naziv");

                entity.HasOne(d => d.IzdavačkaKuća)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.IzdavačkaKućaId)
                    .HasConstraintName("album_to_izdavacka_kuca");

                entity.HasOne(d => d.Izvođač)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.IzvođačId)
                    .HasConstraintName("album_to_band_id");
            });

            modelBuilder.Entity<AlbumKorisnik>(entity =>
            {
                entity.ToTable("Album-Korisnik");

                entity.HasIndex(e => e.AlbumId, "fki_Album-Korisnik_to_album");

                entity.HasIndex(e => e.KorisnikId, "fki_Album-Korisnik_to_korisnik");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AlbumId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("album_id");

                entity.Property(e => e.Cijena).HasColumnName("cijena");

                entity.Property(e => e.KorisnikId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("korisnik_id");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumKorisniks)
                    .HasForeignKey(d => d.AlbumId)
                    .HasConstraintName("Album-Korisnik_to_album");

                entity.HasOne(d => d.Korisnik)
                    .WithMany(p => p.AlbumKorisniks)
                    .HasForeignKey(d => d.KorisnikId)
                    .HasConstraintName("Album-Korisnik_to_korisnik");
            });

            modelBuilder.Entity<Band>(entity =>
            {
                entity.ToTable("Band");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Glazbenik>(entity =>
            {
                entity.ToTable("Glazbenik");

                entity.HasIndex(e => e.MjestoId, "fki_glazbenik_to_mjesto");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DatumRođenja)
                    .HasColumnType("date")
                    .HasColumnName("datum_rođenja");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ime");

                entity.Property(e => e.MjestoId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("mjesto_id");

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("prezime");

                entity.HasOne(d => d.Mjesto)
                    .WithMany(p => p.Glazbeniks)
                    .HasForeignKey(d => d.MjestoId)
                    .HasConstraintName("glazbenik_to_mjesto");
            });

            modelBuilder.Entity<IzdavačkaKuća>(entity =>
            {
                entity.ToTable("Izdavačka_kuća");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ime");
            });

            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.ToTable("Korisnik");

                entity.HasIndex(e => e.MjestoId, "fki_korinik_to_mjesto_rodjenja");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ime");

                entity.Property(e => e.MjestoId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("mjesto_id");

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("prezime");

                entity.HasOne(d => d.Mjesto)
                    .WithMany(p => p.Korisniks)
                    .HasForeignKey(d => d.MjestoId)
                    .HasConstraintName("korinik_to_mjesto_rodjenja");
            });

            modelBuilder.Entity<MjestoRođenja>(entity =>
            {
                entity.ToTable("Mjesto_rođenja");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ime");
            });

            modelBuilder.Entity<Pjesma>(entity =>
            {
                entity.ToTable("Pjesma");

                entity.HasIndex(e => e.AlbumId, "fki_pjesma_to_album");

                entity.HasIndex(e => e.ZanrId, "fki_pjesma_to_zanr");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AlbumId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("album_id");

                entity.Property(e => e.GodinaIzdavanja)
                    .HasColumnType("date")
                    .HasColumnName("godina_izdavanja");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("naziv");

                entity.Property(e => e.ZanrId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("zanr_id");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Pjesmas)
                    .HasForeignKey(d => d.AlbumId)
                    .HasConstraintName("pjesma_to_album");

                entity.HasOne(d => d.Zanr)
                    .WithMany(p => p.Pjesmas)
                    .HasForeignKey(d => d.ZanrId)
                    .HasConstraintName("pjesma_to_zanr");
            });

            modelBuilder.Entity<PjesmaKorisnik>(entity =>
            {
                entity.ToTable("Pjesma-Korisnik");

                entity.HasIndex(e => e.KorisnikId, "fki_pjesma_korisnik_to_korisnik");

                entity.HasIndex(e => e.PjesmaId, "fki_pjesma_korisnik_to_pjesma");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cijena).HasColumnName("cijena");

                entity.Property(e => e.KorisnikId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("korisnik_id");

                entity.Property(e => e.PjesmaId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("pjesma_id");

                entity.HasOne(d => d.Korisnik)
                    .WithMany(p => p.PjesmaKorisniks)
                    .HasForeignKey(d => d.KorisnikId)
                    .HasConstraintName("pjesma_korisnik_to_korisnik");

                entity.HasOne(d => d.Pjesma)
                    .WithMany(p => p.PjesmaKorisniks)
                    .HasForeignKey(d => d.PjesmaId)
                    .HasConstraintName("pjesma_korisnik_to_pjesma");
            });

            modelBuilder.Entity<ČlanoviBendum>(entity =>
            {
                entity.ToTable("Članovi_benda");

                entity.HasIndex(e => e.BandId, "fki_clanovi_benda_to_band");

                entity.HasIndex(e => e.GlazbenikId, "fki_clanovi_benda_to_glazbenik");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BandId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("band_id");

                entity.Property(e => e.Datum)
                    .HasColumnType("date")
                    .HasColumnName("datum");

                entity.Property(e => e.GlazbenikId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("glazbenik_id");

                entity.Property(e => e.Instrument)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("instrument");

                entity.HasOne(d => d.Band)
                    .WithMany(p => p.ČlanoviBenda)
                    .HasForeignKey(d => d.BandId)
                    .HasConstraintName("clanovi_benda_to_band");

                entity.HasOne(d => d.Glazbenik)
                    .WithMany(p => p.ČlanoviBenda)
                    .HasForeignKey(d => d.GlazbenikId)
                    .HasConstraintName("clanovi_benda_to_glazbenik");
            });

            modelBuilder.Entity<Žanr>(entity =>
            {
                entity.ToTable("Žanr");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
