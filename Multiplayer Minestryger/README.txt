Denne version af Multiplayer Minestryger kan kun k�re lokalt da endpoints er sat til localhost.
For at v�re i stand til at k�re hosten, skal Microsoft SQL Server v�re installeret p� computeren.
Der skal v�re en database med navnet "dmab0917_1067353".
N�r databasen er lavet bruges MineSweeperSQL.sql som ligger i DatabaseScript mappen til at lave de n�dvendige tables.
Dette script kan ogs� bruges til at nulstille databasen.

N�r du er klar til at k�re programmet skal du f�rst starte hosten hvorefter du kan starte et vilk�rligt antal klienter.

OBS!
Klienten laver en ny regel i windows firewall, som �bner for port 80.
Hvis en anden bruges kan det v�re muligt at klienten ikke kan k�re ordentligt

Webklienten skal startes gennem Visual Studio.
Projektet "MineSweeper.WebClient" skal s�ttes som startup projekt og skal startes mens hosten k�rer.