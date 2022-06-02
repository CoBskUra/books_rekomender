import pandas as pd
from collections import Counter
from ast import literal_eval
import sys

colection_to_count = ["title",	"publisher", "target_groups", "language_code",	"country_of_origin"]
colectiton_to_take_med = ["num_pages",	"publication_date",	"average_rating",	"ratings_count",	"month_rentals"]
hamingowe = ["tags", "Genres", "authors"]

class averageBook:
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

df = pd.DataFrame(columns=['bookID', 'title', 'authors', 'publisher', 'Genres', 'target_groups', 'tags',
                           'num_pages', 'language_code', 'country_of_origin', 'publication_date', 'average_rating',
                           'ratings_count', 'month_rentals'])
numberOfBooks = (len(sys.argv)-1)/14
for x in range(int(numberOfBooks)):
    df2 = {'bookID': sys.argv[14*x+1], 'title': sys.argv[14*x+2], 'authors': sys.argv[14*x+3], 'publisher': sys.argv[14*x+4],
           'Genres': sys.argv[14*x+5], 'target_groups': sys.argv[14*x+6], 'tags': sys.argv[14*x+7],
           'num_pages': sys.argv[14*x+8], 'language_code': sys.argv[14*x+9], 'country_of_origin': sys.argv[14*x+10],
           'publication_date': sys.argv[14*x+11], 'average_rating': sys.argv[14*x+12], 'ratings_count': sys.argv[14*x+13],
           'month_rentals': sys.argv[14*x+14]}
    df = df.append(df2, ignore_index=True)
else:
    recs = averageBook.avr_book(df)
    pd.options.display.max_colwidth = 999
    print(recs)