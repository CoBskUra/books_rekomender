# Content-based books recommendation system
Projekt z WSI. System rekomendacyjny content-based.



additional_data_base_info - zawiera zmienne kolumn które trzeba dodać do bazy danych

books - przerobiona baza danych 

orginal_df_books - baza danych z książkami pobrana z https://www.kaggle.com/datasets/jealousleopard/goodreadsbooks?resource=download

Average_Book - stworzona biblioteka która pozwala na znajdowanie "średniej książki". Po wprowadzeniu bazy danych księżek zwraca książkę która
    łączy najwięcej cech z wprowadzonych książek.
    UŻYCIE: 
        import avr_book
        avr_book.avr_book(DataFrame)

average_book_testing - testuje poszczególne elementy jak i cały algorytm

Recomender - po wprowadzeniu bazydanych użytkownika (rozumiem jako wybrane wiersze z bazy books.csv) zwraca dataframe nieprzeczytanych książek który posiada dodatkowe kolumny które określają stopień podobieństwa do wybranej książki ( książka porównywana jest w nazwie kolumny, jest to id książki). Posiada też dodatkową kolumne "Sum_metrics" która sumuje podobieństwa książki nieprzeczytanej z wszystkimi przeczytanymi.
    UŻYCIE:
        import Recomender
        unread = Recomender.Recommend(userDataBase)

        # bierzemy bookID dowolnej książki
        user_book_id = userDataBase.iloc[0]["bookID"]

        # otrzymujemy książki posegregowane malejąco według podobieństwa do książki o id user_book_id
        unread = unread.sort_values(user_book_id, ascending=False)

        # teraz możemy wybrać n najbardziej popularnych książek do książki o id user_book_id
        n = 5
        unread[:n]

recomender_testing - testowanie poszczególnych części jak i całości algorytmu recomender