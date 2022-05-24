import pandas as pd
from collections import Counter
from ast import literal_eval

colection_to_count = ["title", "authors",	"publisher", "target_groups", "language_code",	"country_of_origin"]
colectiton_to_take_med = ["num_pages",	"publication_date",	"average_rating",	"ratings_count",	"month_rentals"]
hamingowe = ["tags", "Genres"]

def avr_book(users_books: pd.DataFrame):
    avr_book = users_books.iloc[0]
    
    for columnName in colection_to_count:
        result = Counter(users_books[columnName].values).most_common(1)
        avr_book[columnName] = result[0][0]
    
    

    for columnName in colectiton_to_take_med:
        avr_book[columnName] = users_books[columnName].median()

    

    for columnName in hamingowe:
        cos = [i for a in users_books[columnName].apply(literal_eval) for i in a]
        countered_cos = Counter(cos).most_common()
        max = countered_cos[0][1]
        out = []
        for a, i in countered_cos:
            if( i >= max):
                out.append(a)
            else:
                break
        avr_book[columnName] = out
    return avr_book