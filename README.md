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

On peut aussi filtrer les sites par nom ou par serveur en utilisant la barre de recherche. *(en haut à gauche)*

![Website List Filtered](./Images/Screenshot%20List%20Filtered.png)
![Website List Filtered Server](./Images/Screenshot%20List%20Filtered%20Server.png)

### Ajouter des sites

On peut rajouter des sites supplémentaires à la liste pour le tester en mettant l'URL du site dans la barre de recherche
puis en appuyant sur la touche `Enter` ou `Entrée`.

![Website List New Website](./Images/Screenshot%20List%20New%20Website.png)
![Website List Added Website](./Images/Screenshot%20List%20Added%20Website.png)

Les sites rajoutés sont aussitôt testé et les statistiques et graphes sont mises à jour.
Les sites sont sauvegardés dans des fichiers JSON qui seront aussi chargés pour les prochaines fois.

### Sauvegarde des données

La liste des sites, les statistiques et les informations des sites sont sauvegardés dans des fichiers JSON.

* [Sites par défaut JSON](./websites.json)
* [Type de Servers JSON](./servers.json)
* [Age des Sites JSON](./ages.json)
* [Nombre de Cookies JSON](./cookies.json)

Les sites par défaut sont utilisés comme base pour les statistiques et les informations des sites. Ils ont été
choisis comme étant les 50 sites les plus visités au monde.

## Fonctionnement

Il y a trois programmes qui sont utilisés pour faire les requêtes et récupérer les statistiques et un programme qui sert
de serveur web pour afficher les pages web. Tous les programmes sont écrits en C# et utilisent le framework .NET Desktop

### [HttpServer](./HttpServer/WebApp.cs)

Ce programme permet d'écouter les requêtes HTTP et de renvoyer les pages web associées. Mais il sert aussi de server
api REST, car grâce à une class [Router](./HttpServer/Router.cs), on peut associer une fonction à une route. Dans ce
cas, il y a trois routes, une par type de statistique. Ces routes sont :

- /api/server
- /api/age
- /api/cookie

Chaque route renvoie les données en JSON et sont aussi capable de gérer les paramètres de la requête. Par exemple, pour
la route /api/server, on peut ajouter un paramètre `?url=chat.openai.com` pour calculer les statistiques juste pour ce
site ou encore `?url=chat.openai.com google.com facebook.com` pour en calculer pour plusieurs sites.
Par défaut, les statistiques sont calculées pour tous les sites se trouvant dans le
fichier [websites.json](./websites.json) mais les sites étant déjà testés sont ignorés et reprend les résultats de la
fois précédente pour plus de rapidité.

### Programs utilisés pour les statistiques

- [ServerStats](./ServerStats/ServerTypes.cs)
- [AgeStats](./AgeStats/PageAge.cs)
- [CookieStats](./CookieStats/CookieCounter.cs)

Le fonctionnement de ces programmes sont très similaire. Ils utilisent tous les mêmes url initiale pour faire les
requêtes et récupérer les données. Ils peuvent tous être lancés depuis le HttpServer en utilisant la route ou en
l'exécutant directement. De la même manière, ils peuvent tous être lancés avec des paramètres ou non pour calculer les
statistiques pour un ou plusieurs sites. Deux d'entre eux utilisent un lien `/sitemap.xml` pour récupérer les liens des
sous-pages du site afin de faire des tests sur le nombre de cookies et l'âge des pages.

## Problèmes rencontrés

![Website List N/A](./Images/Screenshot%20List%20Unknown.png)

Comme visible sur l'image ci-dessus, il y a des sites qui ne fonctionnent pas. Cela est dû au fait que la plupart des
sites ne proposent pas les informations demandées dans les Headers HTTP *(Server et Last-Modified)*. Et aussi la
majorité des sites ne proposent pas de lien vers un fichier `sitemap.xml` qui permet de récupérer les liens des
sous-pages du site. Dans ce cas, il faudrait utiliser un web crawler pour récupérer les liens des sous-pages du site,
mais ceci est illégal et donc pas recommandé.

## Auteur

[Matis HERRMANN](https://github.com/MatisPrograms)