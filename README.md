# WebServiceStatistics

Date de rendu : 14/04/2023

## Description

Produire des statistiques dans le cadre des échanges avec un serveur web en exploitation des champs d'en-tête HTTP.

* **Question 1 :** Créez une liste d'adresses de serveurs Web non sensibles.
  Créez une application qui fait une requête par serveur, récupère la réponse de chaque serveur et fournit des
  statistiques sur la popularité des différents types de serveurs utilisés (ex. Apache, IIS, ...)

* **Question 2 :** Créez une application qui fait plusieurs requêtes vers des pages Web différentes d'un serveur Web non
  sensible.
  Calculer la moyenne et l'écart type de l'âge de ces pages.

* **Question 3 :** Rajouter des programmes qui implémentent des scénarios de test et affiches des résultats statistiques
  qui vous semblent pertinents (a minima 3 qui seront réutilisés dans la question suivante). Votre originalité et
  créativité est un facteur important dans cette question. Vous devez alors écrire dans le README qui accompagnera la
  question 4 à rendre, les use cases techniques qui motivent ces scénarios de test.

* **Question 4 :** Faire une page Web Dynamique (Cf. Cours Middleware et Service oriented Computing du S7) permettant de
  déclencher l'ensemble des programmes déjà écrits implémentant les scénarios de test des questions 1,2,3 et d'afficher
  les résultats statistiques retournés.

## Lancement

Exécuter le fichier [HttpServer.exe](./HttpServer/bin/Release) et ouvrir le lien http://localhost:8080/home

### Statistiques des sites

![Chart Statistics](./Images/Screenshot%20Charts.png)

Il y a trois graphes qui sont affichés :

- Le nombre de sites testé par serveur *(à Gauche)*
- La moyenne, l'écart type de l'âge des sites et le nombre de pages testé *(en haut à droite)*
- La moyenne, l'écart type du nombre de cookies et le nombre de pages testé *(en bas à droite)*

### List des sites

![Website List](./Images/Screenshot%20List.png)

Il y a une liste des sites testés avec les informations suivantes :

- Le nom du site
- L'URL du site
- Le serveur utilisé
- L'âge du site moyen *(en heure)*
- L'écart type de l'âge du site *(en heure)*
- Le nombre de cookies moyen

### Filtrer les sites

On peut également filtrer les sites par nom ou par serveur en utilisant la barre de recherche. *(en haut à gauche)*

![Website List Filtered](./Images/Screenshot%20List%20Filtered.png)
![Website List Filtered Server](./Images/Screenshot%20List%20Filtered%20Server.png)

### Ajouter des sites

On peut rajouter des sites supplémentaires à la liste pour les tester en saisissant l'URL du site dans la barre de
recherche, puis en appuyant sur la touche `Enter` ou `Entrée`.

![Website List New Website](./Images/Screenshot%20List%20New%20Website.png)
![Website List Added Website](./Images/Screenshot%20List%20Added%20Website.png)

Les sites rajoutés sont testés immédiatement et les statistiques ainsi que les graphes sont mis à jour. De plus, les
sites sont sauvegardés dans des fichiers JSON qui seront chargés automatiquement lors des prochaines utilisations du
programme.

### Sauvegarde des données

La liste des sites, les statistiques et les informations des sites sont sauvegardés dans des fichiers JSON.

* [Sites par défaut JSON](./websites.json)
* [Type de Servers JSON](./servers.json)
* [Age des Sites JSON](./ages.json)
* [Nombre de Cookies JSON](./cookies.json)

Les sites par défaut sont utilisés comme base pour les statistiques et les informations des sites. Ils ont été choisis
comme étant les 50 sites les plus visités au monde.

## Fonctionnement

Il y a trois programmes utilisés pour effectuer les requêtes et récupérer les statistiques, ainsi qu'un programme qui
sert de serveur web pour afficher les pages web. Tous ces programmes sont écrits en C# et utilisent le framework .NET
Desktop.

### [HttpServer](./HttpServer/WebApp.cs)

Ce programme permet d'écouter les requêtes HTTP et de renvoyer les pages web associées. Il sert également de serveur API
REST car il est possible d'associer une fonction à une route grâce à la classe [Router](./HttpServer/Router.cs). Dans ce
cas, il y a trois routes, une pour chaque type de statistique. Ces routes sont :

- /api/server
- /api/age
- /api/cookie

Chaque route renvoie les données au format JSON et est également capable de gérer les paramètres de la requête. Par
exemple, pour la route /api/server, il est possible d'ajouter un paramètre `?url=chat.openai.com` pour calculer les
statistiques uniquement pour ce site, ou encore `?url=chat.openai.com google.com facebook.com` pour en calculer
plusieurs d'un coup.

Par défaut, les statistiques sont calculées pour tous les sites présents dans le
fichier [websites.json](./websites.json) Cependant, les sites ayant déjà été testés sont ignorés et les résultats
précédemment obtenus sont réutilisés pour améliorer la rapidité des tests.

### Programs utilisés pour les statistiques

- [ServerStats](./ServerStats/ServerTypes.cs)
- [AgeStats](./AgeStats/PageAge.cs)
- [CookieStats](./CookieStats/CookieCounter.cs)

Le fonctionnement de ces programmes est très similaire. Ils utilisent tous les mêmes URL initiales pour effectuer les
requêtes et récupérer les données. Ils peuvent être lancés depuis le serveur HTTP en utilisant la route correspondante
ou exécutée directement. De même, ils peuvent tous être lancés avec ou sans paramètres pour calculer les statistiques
pour un ou plusieurs sites. Certains d'entre eux utilisent un lien `sitemap.xml` pour récupérer les liens des sous-pages
du site afin de réaliser des tests sur le nombre de cookies et l'âge des pages.

## Métriques choisies

Comme pour la question 2, j'ai choisi de faire des statistiques sur les Cookies utilisés.
Il y a trois métriques qui sont calculées :

- Le nombre de cookies moyen
- L'écart type du nombre de cookies
- Le nombre de pages testé

Les métriques permettent d'évaluer l'utilisation de cookies sur un site web, ainsi que la stabilité de leur nombre au
fil du temps. Une quantité élevée et stable de cookies peut constituer un problème de confidentialité. La comparaison
des données entre différents sites web peut également révéler des variations significatives dans l'utilisation de
cookies, ce qui peut indiquer une collecte disproportionnée de données personnelles.

## Problèmes rencontrés

![Website List N/A](./Images/Screenshot%20List%20Unknown.png)

Comme le montre l'image ci-dessus, certains sites ne sont pas accessibles. Ce problème résulte du fait que la plupart
des sites ne fournissent pas les informations requises dans les en-têtes HTTP (tels que les champs **Server** et
Last-Modified**). En outre, la majorité des sites ne proposent pas de lien vers un fichier `sitemap.xml` qui permet de
récupérer les liens des sous-pages du site. Dans de tels cas, il est possible de recourir à un robot d'exploration web
pour extraire ces liens. Toutefois, cette pratique est illégale et ne saurait être recommandée.

## Auteur

[Matis HERRMANN](https://github.com/MatisPrograms)