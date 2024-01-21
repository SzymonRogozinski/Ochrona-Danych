# Prezentacja Projektu na przedmiot ODwSI  

## Prolog

Po nierównej walce z Dockerem, nie udało mi się utworzyć kontenerów, dla wszystkich komponentów. Zatem jestem zmuszony bronić projekt w wersji deweloperskiej, uruchamianej przy pomocy Microsoft Visual Studio 2022. W poniższym dokumencie spróbuję, jak najlepiej przedstawić swoją pracę oraz pokazać sposób jej uruchomienia. Za wszelkie utrudnienia, przepraszam.

## Wstęp

Projekt składa się z aplikacji klienckiej, części serwerowej oraz bazy danych. Cały system ma służyć fikcyjnemu bankowi *ISafeBank* do obsługi swoich klientów. Celem projektu było stworzenie systemu bezpiecznego i odpornego na ataki. W dalszej części przedstawię opis poszczególnych komponentów wraz z podpunktami, które realizują.

## Jak uruchomić projekt?

*Opis ten dotyczy uruchomienia projektu przy użyciu Visual Studion 2022 (VS2022).*  
System został napisany przy wykorzystaniu języka C#, więc przed uruchomieniem należy się upewnić, że środowisko zawiera pakiety wspierające ten język.  
Pliki projektów zawierają w sobie wymagane bibilioteki, więc nie wymagają dodatkowej uwagi.  
Przed uruchomieniem aplikacji serwerowej, należy skonfigurować połączenie z bazą danych. W pliku **appsettings.json**, znajdującym się w folderze **BankAPI** umieszczona jest konfiguracja serwera.W polu **ConnectionStrings.DefaultConnection** należy ustawić połączenie z bazą danych. Prosta konfiguracja wygląda w następujący sposób:  
*"server=\<Nazwa serwera\>;database=\<Nazwa bazy danych\>"*  
Po ustawieniu połączenia z bazą danych należy wprowadzić wartości początkowe przy użyciu migracji dostępnych w *EntityFrameworkCore.Tools*. Należy otworzyć konsole pakietów (View->Other Windows->Package Manager Console) i wprowadzić w nią dwie komendy:  
*Add-Migration* *"\<Nazwa migracji\>"*  
*Update-Database*  
Jeśli wszystko poszło dobrze, system jest gotowy do działania.

## Baza danych

W projekcie wykorzystałem Microsoft SQL Server, do utworzenia serwera wraz z bazą danych.  
Baza zawiera cztery tabelę. **Account** zawiera dane wszystkich użytkowników, w tym dane prywatne/wrażliwe oraz hash hasła. **AccountStatus** zawiera informacje o aktualnym stanie konta użytkownika (Zbanowany, czasowo zablokowany, okej), informację o liczbie prób zalogowania oraz datę związaną ze zmianem statusu. **Transfers** opisuję przelewy, które odbyły się w aplikacji. **Logs** zawiera wpisy na temat połączeń do API. Zawierają informację o wykonywanej akcji/endpoint'cie, jaki użytkownik dokonał połączenia oraz o wyniku połączenia.  
Zawartość bazy szyfruję z poziomu serwera. Część danych, którą uważam za wrażliwą, serializuję do postaci *JSON*, a następnie szyfruję ją algorytmem *AES*. Pozostałe dane pozostają bez zmian. Wpis w takiej postaci trafia do bazy danych. Wymusza to szyfrowania danych na wejściu i odszyfrowywania przy ich pobieraniu.

## Serwer

Serwer został napisany przy wykorzystaniu frameworków EntityFrameworkCore oraz ASPNetCore. Konfiguracja serwera zawarta w pliku program.cs opisuję:

1. Składniki, które zostaną wstrzyknięte do środka za pomocą mechanizmu *Dependency Injection*.
2. Wyłączenie nagłówka "Server" odpowiedzi.
3. Realizację połączenia z bazą danych.
4. Politykę *CORS*.
5. Ustawienia autentykacji w postaci żetonów JWT.

W aplikacji opisano 3 kontrolery obsługujące żądania:

1. AuthorizationController - Jego celem jest obsługa kont użytkowników. Logowanie, zmiana hasła, reset hasła. Znajduję się tu też Endpoint pozwalający na wyświetlanie logów, jednakże jest dostępny tylko dla administracji.
2. BankController - Jego celem jest dostarczenie użytkownikom usług bankowych, takich jak widok przelewów, wykonanie przelewu, podejrzenie danych wrażliwych. Wszystkie endpointy wymagają, aby użytkownik był zalogowany.
3. HoneyPot - Jego celem jest wyłapanie podejrzanej aktywności, a nawet nałożenie blokady (Ban) na konto użytkownika o niecnych zamiarach. Udostępniono 11 fałszywych endpointów.

W projekcie znajduję się też klasa **DataContext**. Służy ona do zdefiniowania połączenia z bazą danych, a dokładnia za mapowanie obiektów C# na rekord w tabeli (i odwrotnie). Klasa ta, też zawiera metodę **OnModelCreating**, która odpowiada za wpisanie danych początkowych do bazy danych (Code first).  
Pomimo iż stanowi to podatność, ponieważ otrzymanie kodu źródłowego serwera ujawni dane użytkowników tam podanych, musiałem pozostawić to w kodzie, aby dało się odtworzyć te dane na innej maszynie.  
Rozwiązania tego problemu są dwa, albo nie korzystać z metody Code first, albo po migracji, usunąć metody dodające dane (jednak należy pamiętać, że wywołanie migracji, spowoduję utracenie tych danych).

## Aplikacja Kliencka

Aplikacja kliencka została napisana przy wykorzystaniu Frameworku Blazor, a konkretniej Blazor WebAssembly. Jest ona w postaci aplikacji internetowej SPA. Używa ona domyślnego szkieletu tego typu aplikacji.  
Aplikacja trzyma otrzymany żeton w pamięci (Klasa Token Holder) i dodaję go do żądań http.

## Biblioteka Shared

Jest to trzeci projekt C#, który w przeciwieństwie do innych, nie może być samodzielnie uruchamiany. Zawiera ona klasy, służace do trzymania danych oraz dane służące do kontaktu między Klientem a Serwerem. Znajdują się tu też klasy, służące Aplikacji klienckiej do kontaktu z serwerem oraz klasa sprawdzająca siłę hasła.

## Dodatkowe uwagi


### Loginy i hasła użytkowników
W bazie istnieją trzy konta:

1. BasiaK6 - GaFzHOtFuPKBSa
2. Konon111 - DdngMLAnt1G4gA
3. Administrator - eGG8Q9OQRjOxS*Bn

Hasła te są słabe w rozumieniu funkcji sprawdzającej hasło, są zbyt słabe, jednak pełnią one rolę *placeholdera*, który należyzastąpić po zainicjalizowaniu bazy danych.  
Udostępniam, je wiedząc, że nie zostaną wykorzystane w złych celach. :)

### Pomiar siły hasła

Pomiar siły hasłą jest mierzony dwuetapowo.  
Pierw jest mierzony potencjał hasła wynikający z jego długości. Wzór to  $log(r^n)$ przekształcone do $n*log(r)$. n to długość hasła, a r to liczba możliwych do wpisania znaków (wszystkie znaki drukowalne i polskie litery, 112 znaków).  
Drugim etapem jest pomiar entropii samego słowa. Wzorem $\sum_{k=1}^np(x_i)*log(\frac{1}{p(x_i)})$. Następnie przy użyciu normalizacji min-max, sprowadzam wynik do wartości od zera do jeden.  
Ostatecznie wynik (E) wynosi iloczy wyników powyższych dwóch etapów. Wynik liczbowy zamienia się na słowny poprzez porównanie z tabelą:

- Very weak dla E < 40
- Weak dla 40 < E < 60
- Medium dla 60 < E < 90
- Strong dla 90 < E < 120
- Very strong dla E > 120

Ostateczna uwaga. Jeśli hasło nie zawiera dużych i małych liter, znaków specjalnych, liczb lub jest krótsze niż 8 znaków, jest uważane za hasło bardzo słabe, a zatem nieakceptowalne.


## Bibliografia

Skąd pozyskałem wiedzę:

- Wykład Programowanie aplikacji mobilnych i webowych u Pana Dr inż. Radosława Roszczyka.
- Wykład Ochrona danych w systemach informatycznych u Pana Dr hab. inż. Dariusza Sawickiego.
- Laboratoria Ochrona danych w systemach informatycznych u Pana Dr inż. Bartosza Chabra.
- Dużo wpisów na forach m.in. StackOverflow.
- Dziesiątki stron dokumentacji Microsoftu.
- Kilkanaście godzin wideo-poradników na YouTube.

Pracę wykonał *Szymon Rogoziński*
