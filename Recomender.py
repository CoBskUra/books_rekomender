import pandas as pd
import math
from ast import literal_eval

def Make_List_From_txt(title: str):
    genres = open('additional_data_base_info/' + title + '.txt','r')
    genres_list =[]
    for line in genres:
        genres_list.append(line.strip())
    return genres_list

def ToHamming(all_possibilities, vector):
    result = []
    for x in all_possibilities:
        if(x in vector):
            result.append(1)
        else:
            result.append(0)
    return result

def Haming_Similaryty(vector1:list, vector2:list):
    wynik = 0
    if(len(vector1) != len(vector2)):
        return -1
    
    for i in range(0, len(vector1)-1):
        if(vector1[i] == vector2[i]):
            wynik = wynik +1
    return wynik

def CosSimilarity(vector_1, book_vector, max_num_pages, max_publicate_data):
    columns_to_cos_metrice = ["publisher", "target_groups", "num_pages", "language_code", "country_of_origin", "publication_date"]
    maxs = {"num_pages": max_num_pages, "publication_date": max_publicate_data}
    squer_sum_bookVector = 0
    squer_sum_vector = 0
    vector_1__multiplay__book_vector = 0

    squer_sum_bookVector += 1
    if any( item in literal_eval(vector_1["authors"]) for item in literal_eval(book_vector["authors"]) ):
        vector_1__multiplay__book_vector += 1
        squer_sum_vector += 1
    for name in columns_to_cos_metrice:
        if(type(vector_1[name]) == str):
            squer_sum_bookVector += 1
            if(vector_1[name] == book_vector[name]):
                vector_1__multiplay__book_vector += 1
                squer_sum_vector += 1
        else:
            squer_sum_bookVector += math.pow(book_vector[name] / maxs[name],2)
            squer_sum_vector +=  math.pow(vector_1[name] / maxs[name],2)
            vector_1__multiplay__book_vector += (book_vector[name] * vector_1[name])/(maxs[name]*maxs[name])
    return vector_1__multiplay__book_vector/(math.sqrt(squer_sum_vector*squer_sum_bookVector))

def Recommend(user_read_books: pd.DataFrame):
    # pobranie danych
    lirabary = pd.read_csv('books.csv', on_bad_lines='skip')
    
    # # usunięcie księżek przeczytanych
    cond = lirabary["bookID"].isin(user_read_books["bookID"])
    lirabary = lirabary.loc[~cond]

    # pobranie listy gatunków i tagów
    genres = Make_List_From_txt("genres")
    tags = Make_List_From_txt("tags")
    
    # wyznaczenie maksimów
    max_date = lirabary["publication_date"].max()
    max_pages = lirabary["num_pages"].max()

    # obliczenie ilości gatunków i tagów
    genres_lenght = len(genres)
    tags_lenght = len(tags)

    # obliczanie podobieństwa
    for index, row in user_read_books.iterrows():
        lirabary[row["bookID"]] =  lirabary.apply(lambda x:
            CosSimilarity(x, row, max_pages,max_date) + 
            Haming_Similaryty(ToHamming(genres, x["Genres"]), ToHamming(genres, row["Genres"]))/genres_lenght +
            Haming_Similaryty(ToHamming(tags, x["tags"]), ToHamming(tags,row["tags"]))/tags_lenght , axis=1)
    

    bookid = user_read_books["bookID"]
    lirabary["Sum_metrics"] = lirabary[bookid].sum(axis=1)

    return lirabary


