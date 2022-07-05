# Einstiegsseite
Die Einstiegsseite besteht aus einer Karte und einer Suchmaske.

## Karte
Zu Beginn zeigt die Karte alle Bohrungen im Kanton Solothurn an. Um in der Karte zu navigieren stehen die folgenden Werkzeuge zur Verfügung:
* Zoom In / Zoom Out ![Zoom In / Zoom Out Icon](../images/zoom-icon.png)
* Ansicht gesamter Kanton ![Ansicht gesamter Kanton Icon](../images/all-out-icon.png)
* Zurück zur letzten Ansicht ![Zurück zur letzten Ansicht Icon](../images/back-icon.png)

Das Verschieben des Kartenausschnitts geschieht durch Klicken und Ziehen im Kartenfenster, ohne dass ein Werkzeug ausgewählt werden muss.

Durch Klicken auf eine Bohrung werden die wichtigsten Informationen in einem Popup-Fenster angezeigt, ohne dass ein Werkzeug ausgewählt werden muss.

## Suchmaske
Die Suchmaske besteht aus fünf Eingabefeldern nach denen die Bohrungen bzw. Standorte durchsucht werden können:
* Gemeinde
* Grundbuchnummer(n)
* Bezeichnung
* Erstellungsdatum
* Mutationsdatum

Die gefundenen Standorte werden in einer Tabelle unterhalb der Suchmaske angezeigt. Im Kartenfenster werden nur noch die Bohrungen der gefundenen Standorte angezeigt, der Kartenausschnitt passt sich automatisch den Suchergebnissen an.

## Standorte hinzufügen
Durch Klicken auf den Button 'Neuen Standort hinzufügen' wird ein Dialog geöffnet, in dem ein neuer Standort angelegt werden kann. Hier können die folgenden Felder befüllt werden.

- Bezeichnung (obligatorisch)
- Bemerkung
- Grundbuchnummer

Die folgenden Attribute werden automatisch gesetzt und sind nicht im Dialog sichtbar.

- Gemeinde (sobald eine Bohrung mit Geometrie vorhanden ist)
- Erstellungsdatum
- User bei der Erstellung

## Standorte editieren
In der Tabelle mit gefundenen Standorten unterhalb der Suchmaske können einzelne Standorte editiert werden. Durch Klicken auf das Editieren-Icon ![Editieren-Icon](../images/edit-icon.png) wird ein Dialog geöffnet, in dem der Standort editiert werden kann.

Die folgenden Felder können editiert werden.

- Bezeichnung
- Bemerkung
- Grundbuchnummer

Die folgenden Attribute werden zusätzlich angezeigt, können aber nicht editiert werden.

- Gemeinde
- Erstellungsdatum
- Erstellender User
- Mutationsdatum
- User bei der Mutation
- Datum der Freigabe durch das AfU
- User bei der Freigabe durch das AfU

Ausserdem werden die dem Standort zugeordneten Bohrungen auf einer Karte dargestellt und in einer Tabelle angezeigt.

## Standorte löschen
In der Tabelle mit gefundenen Standorten unterhalb der Suchmaske können einzelne Standorte durch Klicken auf das Löschen-Icon ![Löschen-Icon](../images/delete-icon.png) gelöscht werden.
