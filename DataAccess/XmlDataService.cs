using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GameLibrary.Models;

namespace GameLibrary.DataAccess
{
    // Domain orchestrator: wires per-entity serialization rules into XmlEntityService<T>
    // and handles the single-root <GameLibrary> file structure.
    public class XmlDataService
    {
        private readonly string _filePath;
        private readonly XmlEntityService<Developer> _devService;
        private readonly XmlEntityService<Game> _gameService;
        private readonly XmlEntityService<Player> _playerService;
        private readonly XmlEntityService<Achievement> _achService;

        public XmlDataService(string filePath)
        {
            _filePath = filePath;

            _devService = new XmlEntityService<Developer>(
                sectionElement: "Developers",
                itemElement: "Developer",
                serialize: (w, d) =>
                {
                    w.WriteAttributeString("Id", d.Id.ToString());
                    w.WriteElementString("Name", d.Name);
                    w.WriteElementString("Country", d.Country);
                    w.WriteElementString("FoundedYear", d.FoundedYear.ToString());
                },
                deserialize: x => new Developer
                {
                    Id          = (int?)x.Attribute("Id") ?? 0,
                    Name        = (string?)x.Element("Name") ?? "",
                    Country     = (string?)x.Element("Country") ?? "",
                    FoundedYear = (int?)x.Element("FoundedYear") ?? 0
                });

            _gameService = new XmlEntityService<Game>(
                sectionElement: "Games",
                itemElement: "Game",
                serialize: (w, g) =>
                {
                    w.WriteAttributeString("Id", g.Id.ToString());
                    w.WriteAttributeString("DeveloperId", g.DeveloperId.ToString());
                    w.WriteElementString("Title", g.Title);
                    w.WriteElementString("Genre", g.Genre);
                    w.WriteElementString("Rating", g.Rating.ToString(CultureInfo.InvariantCulture));
                    w.WriteElementString("ReleaseDate", g.ReleaseDate.ToString("O"));
                },
                deserialize: x => new Game
                {
                    Id          = (int?)x.Attribute("Id") ?? 0,
                    DeveloperId = (int?)x.Attribute("DeveloperId") ?? 0,
                    Title       = (string?)x.Element("Title") ?? "",
                    Genre       = (string?)x.Element("Genre") ?? "",
                    Rating      = double.TryParse((string?)x.Element("Rating"), NumberStyles.Any,
                                      CultureInfo.InvariantCulture, out double r) ? r : 0.0,
                    ReleaseDate = DateTime.TryParse((string?)x.Element("ReleaseDate"), out DateTime rd)
                                      ? rd : DateTime.MinValue
                });

            _playerService = new XmlEntityService<Player>(
                sectionElement: "Players",
                itemElement: "Player",
                serialize: (w, p) =>
                {
                    w.WriteAttributeString("Id", p.Id.ToString());
                    w.WriteElementString("Username", p.Username ?? "");
                    w.WriteElementString("Email", p.Email ?? "");
                    w.WriteElementString("RegistrationDate", p.RegistrationDate.ToString("O"));
                    w.WriteStartElement("GameIds");
                    foreach (var gId in p.GameIds ?? new List<int>())
                        w.WriteElementString("GameId", gId.ToString());
                    w.WriteEndElement();
                },
                deserialize: x => new Player
                {
                    Id               = (int?)x.Attribute("Id") ?? 0,
                    Username         = (string?)x.Element("Username") ?? "",
                    Email            = (string?)x.Element("Email") ?? "",
                    RegistrationDate = DateTime.TryParse((string?)x.Element("RegistrationDate"), out DateTime dt)
                                           ? dt : DateTime.MinValue,
                    GameIds          = x.Element("GameIds")
                                        ?.Elements("GameId")
                                        .Select(e => int.TryParse(e.Value, out int id) ? id : 0)
                                        .Where(id => id > 0)
                                        .ToList() ?? new List<int>()
                });

            _achService = new XmlEntityService<Achievement>(
                sectionElement: "Achievements",
                itemElement: "Achievement",
                serialize: (w, a) =>
                {
                    w.WriteAttributeString("Id", a.Id.ToString());
                    w.WriteAttributeString("PlayerId", a.PlayerId.ToString());
                    w.WriteAttributeString("GameId", a.GameId.ToString());
                    w.WriteElementString("Title", a.Title ?? "");
                    w.WriteElementString("Description", a.Description ?? "");
                    w.WriteElementString("UnlockedDate", a.UnlockedDate.ToString("O"));
                },
                deserialize: x => new Achievement
                {
                    Id          = (int?)x.Attribute("Id") ?? 0,
                    PlayerId    = (int?)x.Attribute("PlayerId") ?? 0,
                    GameId      = (int?)x.Attribute("GameId") ?? 0,
                    Title       = (string?)x.Element("Title") ?? "",
                    Description = (string?)x.Element("Description") ?? "",
                    UnlockedDate = DateTime.TryParse((string?)x.Element("UnlockedDate"), out DateTime ud)
                                       ? ud : DateTime.MinValue
                });
        }

        // Writes the full library to XML using XmlWriter (ЛР №2 requirement)
        public void SaveToXml(GameLibraryData data)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using var writer = XmlWriter.Create(_filePath, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("GameLibrary");
            _devService.WriteSection(writer, data.Developers);
            _gameService.WriteSection(writer, data.Games);
            _playerService.WriteSection(writer, data.Players);
            _achService.WriteSection(writer, data.Achievements);
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        // Reads the full library using XDocument (ЛР №2 requirement)
        public GameLibraryData LoadFromXml()
        {
            if (!File.Exists(_filePath)) return new GameLibraryData();
            var doc = XDocument.Load(_filePath);
            return new GameLibraryData
            {
                Developers   = _devService.ReadSection(doc),
                Games        = _gameService.ReadSection(doc),
                Players      = _playerService.ReadSection(doc),
                Achievements = _achService.ReadSection(doc)
            };
        }

        // Alternative load using XmlSerializer for demo (ЛР №2 step 3)
        public GameLibraryData LoadWithXmlSerializer()
        {
            if (!File.Exists(_filePath)) return new GameLibraryData();
            var serializer = new XmlSerializer(typeof(GameLibraryData));
            using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            return (GameLibraryData)serializer.Deserialize(fs)!;
        }
    }
}