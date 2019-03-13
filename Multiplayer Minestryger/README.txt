Denne version af Multiplayer Minestryger kan kun køre lokalt da endpoints er sat til localhost.
For at være i stand til at køre hosten, skal Microsoft SQL Server være installeret på computeren.
Der skal være en database med navnet "dmab0917_1067353".
Når databasen er lavet bruges MineSweeperSQL.sql som ligger i DatabaseScript mappen til at lave de nødvendige tables.
Dette script kan også bruges til at nulstille databasen.

Når du er klar til at køre programmet skal du først starte hosten hvorefter du kan starte et vilkårligt antal klienter.

OBS!
Klienten laver en ny regel i windows firewall, som åbner for port 80.
Hvis en anden bruges kan det være muligt at klienten ikke kan køre ordentligt

Webklienten skal startes gennem Visual Studio.
Projektet "MineSweeper.WebClient" skal sættes som startup projekt og skal startes mens hosten kører.