# UniSearch

UniSearch est un MVP mobile de mots mêlés réalisé dans le cadre de PRO403. Le
projet est un monorepo composé d'une API ASP.NET Core .NET 8 avec MongoDB et
d'une application mobile React Native / Expo en TypeScript.

## État actuel

Les fonctionnalités suivantes sont réellement disponibles dans le dépôt :

- inscription et connexion ; toute inscription publique crée un compte
  `Player` ;
- gestion de sessions JWT et accès séparé selon les rôles `Player` et `Admin` ;
- téléchargement, jeu et validation du seul pack joueur disponible : le pack
  de test, constitué de deux grilles ;
- enregistrement local du pack, des parties terminées et des soumissions en
  attente ; une validation est tentée à la fin de la partie et lorsqu'une
  connexion réseau revient ;
- génération, liste et consultation détaillée des grilles pour
  l'administrateur ;
- persistance MongoDB, conteneurs Docker pour l'API et MongoDB, et endpoint de
  santé `/health`.

Ce qui n'est pas encore disponible :

- packs quotidiens ou historique de packs pour les joueurs ;
- affectation, publication, modification ou suppression de packs par un
  administrateur ;
- possibilité de proposer aux joueurs les grilles générées par un
  administrateur ;
- score, streaks et classement ;
- scénarios de tests métier et d'intégration : les projets de tests existent,
  mais ne contiennent encore que des tests initiaux.

Une grille générée par un administrateur est donc actuellement un brouillon non
affecté à un pack. Le joueur ne peut jouer que les deux grilles du pack `test`.

## Technologies

### API et données

- C# / .NET 8 et ASP.NET Core
- MongoDB
- JWT
- Docker et Docker Compose

### Application mobile

- React Native et Expo
- TypeScript
- AsyncStorage pour les données de jeu locales
- Expo SecureStore pour le jeton de session

## Prérequis

- Git
- Docker et Docker Compose
- Node.js et npm

Il n'est pas nécessaire d'installer .NET ou MongoDB localement : l'API et la
base de données sont exécutées avec Docker.

## Récupérer le projet

```bash
git clone https://git.uha4point0.fr/UHA40/fil-rouge-2026/4.0.3/unisearch.git
cd unisearch
```

## Configurer l'environnement de développement

Les fichiers `.env.*` contiennent des secrets et ne sont pas versionnés. Créez
votre configuration locale à partir de l'exemple :

```bash
cp .env.dev.example .env.dev
```

Renseignez au minimum les valeurs suivantes dans `.env.dev` :

- `MONGODB_URI`, par exemple `mongodb://mongodb:27017/unisearch_dev` ;
- `JWT_SECRET`, une valeur aléatoire d'au moins 32 caractères ;
- `SEED_ENABLED=true` ainsi que `SEED_PLAYER_PASSWORD` et
  `SEED_ADMIN_PASSWORD` pour créer les comptes et les deux grilles de test ;
- `API_PORT`, habituellement `8080` ;
- `CORS_ALLOWED_ORIGINS`, si l'application Expo Web est utilisée.

Le seed crée, seulement s'ils n'existent pas déjà, les comptes `player` et
`admin`. Les mots de passe sont ceux configurés dans `.env.dev`. N'ajoutez
jamais ce fichier à Git.

## Lancer l'API et MongoDB

Depuis la racine du dépôt :

```bash
make up ENV=dev
```

Cette commande démarre l'API et MongoDB au premier plan. Dans un autre
terminal, vérifiez l'état des conteneurs ou l'endpoint de santé :

```bash
make ps ENV=dev
curl http://localhost:8080/health
```

En développement, la documentation Swagger est disponible à l'adresse
`http://localhost:8080/swagger` lorsque `API_PORT=8080`.

Pour arrêter l'environnement :

```bash
make down ENV=dev
```

Les commandes Docker disponibles sont `up`, `down`, `build`, `rebuild`,
`logs`, `ps` et `clean`. La commande `clean` supprime également le volume
MongoDB de l'environnement ciblé.

## Lancer l'application mobile

Créez le fichier de configuration du frontend :

```bash
cp frontend/.env.local.example frontend/.env.local
```

Dans `frontend/.env.local`, définissez `EXPO_PUBLIC_API_URL` vers l'API avec
l'adresse IP locale de la machine qui exécute Docker, par exemple
`http://192.168.1.42:8080`.

N'utilisez pas `localhost` : sur un téléphone physique, cette adresse désigne
le téléphone lui-même, et non votre ordinateur. Le téléphone et l'ordinateur
doivent être connectés au même réseau local.

Installez ensuite les dépendances et démarrez Expo :

```bash
cd frontend
npm ci
npx expo start
```

Commandes utiles :

```bash
npx expo start --android
npx expo start --ios
npx expo start --web
npm run lint
npm run typecheck
```

## Parcours disponibles

### Joueur

Après inscription ou connexion avec un compte `Player`, l'application télécharge
le pack `test`. Ses deux grilles sont conservées dans AsyncStorage, ce qui
permet de les rouvrir si le téléchargement suivant échoue. Le joueur trace les
mots, puis l'application enregistre localement le temps, les tentatives et les
tracés avant de demander la vérification à l'API.

Si le réseau est indisponible, la soumission reste en attente. Elle est tentée
à nouveau lorsqu'une connexion utilisable est détectée. Le backend reste la
source de vérité pour valider une grille.

### Administrateur

Un compte `Admin` peut consulter toutes les grilles, afficher leur détail et
générer une grille carrée de taille comprise entre 4 et 10, à partir d'une liste
de mots et d'une difficulté. Les mots doivent pouvoir être placés dans la
grille ; sinon la création est refusée et rien n'est persisté.

Les grilles générées ne sont affectées à aucun pack et ne sont donc pas visibles
par les joueurs. L'interface ne permet pas encore de gérer ou publier des packs.

## API disponible

Les routes métier sont préfixées par `/api` et les routes joueur ou
administrateur demandent un jeton JWT avec le rôle correspondant.

| Méthode | Route | Rôle | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | Public | Crée un compte joueur. |
| `POST` | `/api/auth/login` | Public | Retourne un jeton JWT. |
| `GET` | `/api/packs/player/test` | Player | Retourne exactement les deux grilles du pack de test. |
| `POST` | `/api/grids/{gridId}/verification` | Player | Vérifie une grille terminée et conserve le dernier résultat validé. |
| `POST` | `/api/grids` | Admin | Génère et persiste une grille non affectée à un pack. |
| `GET` | `/api/grids` | Admin | Liste les grilles. |
| `GET` | `/api/grids/{gridId}` | Admin | Retourne le détail d'une grille. |
| `GET` | `/health` | Public | Vérifie la disponibilité de MongoDB. |

## Tests

L'environnement Docker de test peut être exécuté avec :

```bash
make up ENV=test
```
