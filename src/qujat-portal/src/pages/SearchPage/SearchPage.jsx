import React, {useEffect, useState} from 'react';
import './SearchPage.scss'
import search_icon from '../../assets/img/search_icon.svg'
import ic_search from '../../assets/img/ic_search.svg'
import { Link, useParams } from 'react-router-dom'
import instance from '../../axios/axios.js'
import Footer from '../../components/Footer/Footer.jsx'
import narrow_arrow from '../../assets/img/narrow-arrow.svg'

const SearchPage = () => {
    const { searchQuery } = useParams()
    const [query, setQuery] = useState("");
    const [searchResults, setSearchResults] = useState([]);
    
    console.log(searchQuery)

    const getSearchResults = async (searchQuery) => {
        if (searchQuery) {
            const pageIndex = 0;
            const pageSize = 10;
            const nameSearchPattern = searchQuery;

            const url = `/api/1/frontend/main-page/documents/search?pageIndex=${pageIndex}&pageSize=${pageSize}&nameSearchPattern=${encodeURIComponent(
                nameSearchPattern
            )}`;

            const response = await instance.get(url);
            const allResults = response.data.responseData;

            const filteredResults = allResults.filter(
                (result) =>
                    result.nameKz &&
                    result.nameKz.toLowerCase().includes(searchQuery.toLowerCase())
            );

            setSearchResults(filteredResults);
        } else {
            setSearchResults([]);
        }
    };

    const handleSearchInputChange = (event) => {
        const newQuery = event.target.value;
        setQuery(newQuery);
        getSearchResults(newQuery);
    };

    const debouncedQuery = useDebounce(query, 500);
    function useDebounce(value, delay) {
        const [debouncedValue, setDebouncedValue] = useState(value);

        useEffect(() => {
            const handler = setTimeout(() => {
                setDebouncedValue(value);
            }, delay);

            return () => {
                clearTimeout(handler);
            };
        }, [value, delay]);

        return debouncedValue;
    }

    useEffect(() => {
        if (debouncedQuery) {
            getSearchResults(debouncedQuery);
        } else {
            setSearchResults([]);
        }
    }, [debouncedQuery])

    useEffect(() => {
        if (query) {
            getSearchResults(query);
        } else {
            setSearchResults([]);
        }
    }, [query]);

    useEffect(() => {
        if (searchQuery) {
            setQuery(decodeURIComponent(searchQuery));
        }
    }, [searchQuery]);

    return (
        <div className={'page'}>
            <div className={'page__wrapper'}>
                <div className={'page__header'}>
                    Нәтижелер
                </div>
                <div className="paginated__search-field">
                    <div className="paginated__search-container">
                        <img src={search_icon} alt="Поиск"/>
                        <input
                            type="text"
                            placeholder="Құжаттың атын теріңіз"
                            value={query}
                            onChange={handleSearchInputChange}
                        />
                    </div>
                    {searchResults && searchResults.length > 0 && (
                        <div className="search-dropdown">
                            {searchResults.map((result, index) => (
                                <div key={index} className="paginated__search-item">
                                    <Link
                                        to={`/${result.parentSubcategory.parentCategory.id}/subCategories/${result.parentSubcategory.id}/documents/${result.id}`}
                                        className={'search-link'}
                                    >
                                        <div>
                                            <div
                                                className={'search-link__category'}>{result.parentSubcategory.parentCategory.nameKz}</div>
                                            <div
                                                className={'search-link__subCategory'}>{result.parentSubcategory.nameKz}</div>
                                            <div className={'search-link__document'}>{result.nameKz}</div>
                                        </div>
                                        <img src={narrow_arrow} alt={'arrow'} />
                                    </Link>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
            <Footer/>
        </div>
    );
};

export default SearchPage;
