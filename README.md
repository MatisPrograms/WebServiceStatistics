# WebServiceStatistics

Date de rendu : 14/04/2023

## Description
Produire des statistiques dans le cadre des échanges avec un serveur web en exploitation des champs d'en-tête HTTP.

* **Question 1 :** Créez une liste d'adresses de serveurs Web non sensibles.
Créez une application qui fait une requête par serveur, récupère la réponse de chaque serveur et fournit des statistiques sur la popularité des différents types de serveurs utilisés (ex. Apache, IIS, ...)

* **Question 2 :** Créez une application qui fait plusieurs requêtes vers des pages Web différentes d'un serveur Web non sensible.
Calculer la moyenne et l'écart type de l'âge de ces pages.

* **Question 3 :** Rajouter des programmes qui implémentent des scénarios de test et affiches des résultats statistiques qui vous semblent pertinents (a minima 3 qui seront réutilisés dans la question suivante). Votre originalité et créativité est un facteur important dans cette question. Vous devez alors écrire dans le README qui accompagnera la question 4 à rendre, les use cases techniques qui motivent ces scénarios de test.

* **Question 4 :** Faire une page Web Dynamique (Cf. Cours Middleware et Service oriented Computing du S7) permettant de déclencher l'ensemble des programmes déjà écrits implémentant les scénarios de test des questions 1,2,3 et d'afficher les résultats statistiques retournés.


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

On peut aussi filtrer les sites par nom ou par serveur en utilisant la barre de recherche. *(en haut à gauche)*

![Website List Filtered](./Images/Screenshot%20List%20Filtered.png)
![Website List Filtered](./Images/Screenshot%20List%20Filtered%20Server.png)

On peut rajouter des sites supplémentaires à la liste pour le tester en mettant l'URL du site dans la barre de recherche

![Website List Filtered](./Images/Screenshot%20List%20New%20Website.png)
![Website List Filtered](./Images/Screenshot%20List%20Added%20Website.png)

Les sites rajoutés sont aussi tot testé et les statistiques et graphes sont mises à jour.
Les sites sont sauvegardés dans des fichiers JSON qui seront aussi chargés pour les prochaines fois.

## Sauvegarde des données

La liste des sites, les statistiques et les informations des sites sont sauvegardés dans des fichiers JSON.
* [Sites par défaut JSON](./websites.json)
* [Type de Servers JSON](./servers.json)
* [Age des Sites JSON](./ages.json)
* [Nombre de Cookies JSON](./cookies.json)

Les sites par défaut sont utilisés comme base pour les statistiques et les informations des sites. Ils ont étaient 
choisis comme étant les 50 sites les plus visités au monde.

## Auteur

[Matis HERRMANN](https://github.com/MatisPrograms)

