import pandas as pd
import math
from ast import literal_eval
# from sqlalchemy import false
import os
import sys
from pathlib import Path
# from sympy import li

parent_dir = os.path.dirname(__file__)

class recommending:
    def Make_List_From_txt(title):
        file = open(parent_dir+'/additional_data_base_info/' + title + '.txt','r')
        file_contend = []
        for line in file:
            file_contend.append(line.strip())
        return file_contend

    def ToHamming(all_possibilities, vector):
        result = []
        for x in all_possibilities:
            if(x in vector):
                result.append(1)
            else:
                result.append(0)
        return result

    def HammingSimilarity(vector1, vector2):
        wynik = 0
        if(len(vector1) != len(vector2)):
            return -1
        for i in range(0, len(vector1) - 1):
            if(vector1[i] == vector2[i]):
                wynik = wynik + 1
        return wynik

    def CosSimilarity(vector_1, book_vector, max_num_pages, max_publicate_data):
        columns_to_cos_metrice = ["publisher", "target_groups", "num_pages", "language_code", "country_of_origin", "publication_date"]
        maxs = {"num_pages": max_num_pages, "publication_date": max_publicate_data}
        sqr_sum_bookVector = 0
        sqr_sum_vector = 0
        vector_1__multiply__book_vector = 0
        sqr_sum_bookVector += 1
        if any(item in literal_eval(vector_1["authors"]) for item in literal_eval(book_vector["authors"])):
            vector_1__multiply__book_vector += 1
            sqr_sum_vector += 1
        for name in columns_to_cos_metrice:
            if(type(vector_1[name]) == str):
                sqr_sum_bookVector += 1
                if(vector_1[name] == book_vector[name]):
                    vector_1__multiply__book_vector += 1
                    sqr_sum_vector += 1
            else:
                sqr_sum_bookVector += math.pow(float(book_vector[name])/float(maxs[name]),2)
                sqr_sum_vector +=  math.pow(float(vector_1[name])/float(maxs[name]),2)
                vector_1__multiply__book_vector += (float(book_vector[name]) * float(vector_1[name])) / float((maxs[name]) * float(maxs[name]))
        return vector_1__multiply__book_vector / (math.sqrt(sqr_sum_vector * sqr_sum_bookVector))

    def Recommend(user_read_books: pd.DataFrame):   
        # pobranie danych
        library = pd.read_csv(r"E:\Reposy\books_rekomender\src\WebApp\BooksRecommender\PythonCode\books.csv")
        library["publication_date"] = library["publication_date"].apply(lambda x: math.floor(x / 10))
        # # usunięcie księżek przeczytanych
        #cond = library["bookID"].isin(user_read_books["bookID"])
        #library = library.loc[~cond]
        # pobranie listy gatunków i tagów
        genres = recommending.Make_List_From_txt("genres")
        tags = recommending.Make_List_From_txt("tags")
        # wyznaczenie maksimów
        max_date = library["publication_date"].max()
        max_pages = library["num_pages"].max()
        # obliczenie ilości gatunków i tagów
        genres_length = len(genres)
        tags_length = len(tags)
        # obliczanie podobieństwa
        xd = pd.DataFrame()
        for index, row in user_read_books.iterrows():
            tmp = library.apply(lambda x:
                recommending.CosSimilarity(x, row, max_pages,max_date) + recommending.HammingSimilarity(recommending.ToHamming(genres, x["Genres"]), recommending.ToHamming(genres, row["Genres"])) / genres_length + recommending.HammingSimilarity(recommending.ToHamming(tags, x["tags"]), recommending.ToHamming(tags,row["tags"])) / tags_length , axis=1)
            if(len(xd)==0):
                xd = tmp
            else:
                xd = xd+tmp
        bookid = user_read_books["bookID"]
        library["Sum_metrics"] = xd
        return library

    def TheBest(library, option=0, isUserBase=False):
        rating = "average_rating"
        if(isUserBase):
            rating = "user_rating"
        if(option == 0):
            return library.sort_values(rating, ascending=False)
        if(option == 1):
            library["rating_x_ratings_count"] = library[rating] * library["ratings_count"]
            library = library.sort_values("rating_x_ratings_count", ascending=False)
            return library.drop(["rating_x_ratings_count"], axis=1)
        if(option == 2):
            library["rating_x_month_rentals"] = library[rating] * library["month_rentals"]
            library = library.sort_values("rating_x_month_rentals", ascending=False)
        return library.drop(["rating_x_month_rentals"], axis=1);

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
    recs = recommending.Recommend(df)
    print(recs["Sum_metrics"].values.tolist())