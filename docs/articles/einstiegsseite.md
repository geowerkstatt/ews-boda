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

## Daten exportieren

Unter dem Menüpunkt _Daten exportieren_ ![Daten exportieren](../images/file-download-icon.png) können die Daten in eine Textdatei mit kommagetrennten Werten (CSV) exportiert und heruntergeladen werden. Die exportierten Felder entsprechen denen der Datenbank View _bohrung.data_export_. Die in UTF-8 codierten Daten können anschliessend, bspw. in Excel unter _Daten_ -> _Aus Text/CSV_ geladen werden.
