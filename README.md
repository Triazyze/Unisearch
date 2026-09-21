# UniSearch

UniSearch est une application mobile de mots mêlés.

Chaque jour, le joueur peut accéder à de nouvelles grilles, trouver les mots demandés et faire valider les grilles terminées. L'application peut également conserver une grille terminée lorsqu'aucune connexion Internet n'est disponible afin de la synchroniser lorsque la connexion revient.

L'application possède deux types d'utilisateurs :

Joueur : joue et complète les grilles de mots mêlés.
Administrateur : génère les grilles proposées aux joueurs.

## Technologies

### Backend

* C# / .NET 8
* ASP.NET Core
* MongoDB
* Docker

### Mobile

* React Native
* Expo
* TypeScript

### DevOps

* Docker / Docker Compose
* GitLab CI/CD

## Récupérer le projet

### Prérequis

Pour récupérer et lancer UniSearch, les logiciels suivants doivent être installés sur votre ordinateur :

Git, pour télécharger le projet ;
Docker, pour lancer le serveur et la base de données ;
Node.js et npm, pour lancer l'application mobile.

Il n'est pas nécessaire d'installer .NET ou MongoDB directement sur l'ordinateur : ils sont exécutés avec Docker.

### Télécharger UniSearch

Ouvrez un terminal dans le dossier dans lequel vous souhaitez installer UniSearch.

Clonez ensuite le projet :

```git clone https://git.uha4point0.fr/UHA40/fil-rouge-2026/4.0.3/unisearch.git```

Une fois le téléchargement terminé, entrez dans le dossier du projet :

``cd UniSearch```

Le projet est maintenant présent sur votre ordinateur.

## Lancer UniSearch

Depuis la racine du projet, lancez :

```make up ENV=dev```

Cette commande démarre automatiquement :

le serveur UniSearch ;
la base de données MongoDB.

Docker s'occupe de préparer les différents éléments nécessaires. Le premier lancement peut donc prendre un peu plus de temps. Le projet prévoit bien un Makefile pour centraliser les commandes Docker.
