import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from "react-redux";
import {fetchCategories, setCurrentCategory} from "../../store/slices/categoriesSlice.js";
import { setCurrentSubCategory } from '../../store/slices/subCategoriesSlice.js'
import { setCurrentDocument } from '../../store/slices/documentsSlice.js'
import './HomePage.scss'
import { Button, Pagination } from "@mui/material";
import instance from "../../axios/axios.js";
import search_icon from "../../assets/img/search_icon.svg";
import ic_search from "../../assets/img/ic_search.svg";
import folder from "../../assets/folder.svg";
import file from "../../assets/file-04.svg";
import { Link, useNavigate } from "react-router-dom";
import Footer from "../../components/Footer/Footer.jsx";

const HomePage = () => {
    const [documentCount, setDocumentCount] = useState(0)
    const { categories, totalCategories } = useSelector(state => state.categories)
    const dispatch = useDispatch()
    const [query, setQuery] = useState("");
    const [searchResults, setSearchResults] = useState([]);
    const [page, setPage] = useState(1);
    const itemsPerPage = 10;
    const totalPages = Math.ceil(totalCategories / itemsPerPage);
    const navigate = useNavigate()
    
    const handleChange = (event, value) => {
        setPage(value);
    };

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
  
  const handleSearchInputClick = (event) => {
      if (event.key === 'Enter') {
          navigate(`/search-page/${query}`)
      }
  }

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

    const debouncedQuery = useDebounce(query, 500);
  
    const customButtonStyle = {
        backgroundColor: '#0193EB',
        color: 'white'
    };
    
    const getDocumentCount = async () => {
        const response = await instance.get('/api/1/frontend/main-page/documents/count')
        setDocumentCount(response.data.responseData.documentCount)
    }

  useEffect(() => {
    if (debouncedQuery) {
      getSearchResults(debouncedQuery);
    } else {
      setSearchResults([]);
    }
  }, [debouncedQuery])

    useEffect(() => {
        getDocumentCount()
        dispatch(fetchCategories({
            pageIndex: 0
        }))
    }, [dispatch]);

    useEffect(() => {
        dispatch(fetchCategories({
            pageIndex: page - 1
        }));
    }, [page, dispatch]);
    
    return (
        <div className={'main-wrapper'}>
            <div className={'main-container main-background'}>
                <Button style={customButtonStyle}>
                    {`Барлығы ${documentCount} құжат бар`}
                </Button>
                <span className={'main-text'}>
                    Қазақ тіліндегі құжаттардың автоматтандырылған электрондық базасы
                </span>
                <div className="search-field">
                    <div className="search-container">
                        <img src={search_icon} alt="Поиск"/>    
                        <input
                            type="text"
                            placeholder="Құжаттың атын теріңіз"
                            value={query}
                            onChange={handleSearchInputChange}
                            onKeyPress={handleSearchInputClick}
                        />
                    </div>
                    {searchResults && searchResults.length > 0 && (
                        <div className="search-dropdown">
                            {searchResults.map((result, index) => (
                                <div key={index} className="search-item">
                                    <img src={ic_search} alt="Поиск"/>
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
                                    </Link>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
            <div className={'categories-container'}>
                <div className={'categories-wrapper'}>
                    <span className={'categories-header'}>
                        Барлық категориялар
                    </span>
                    <span className={'categories-subheader'}>
                        Пайдаланушылар жақында қараған құжаттар тізімі
                    </span>
                    <div className={'categories'}>
                        {
                            categories.map(category => (
                                <div key={category.id} className={'category-item'}>
                                <img src={folder} alt={'folder'}/>
                                    <img src={category.iconBlob.uri} className={'img-overlay'} alt='category icon' />
                                    <Link to={`/${category.id}/subCategories`} onClick={() => dispatch(setCurrentCategory(category))} className={'text-overlay'}>
                                        {category.nameKz}
                                    </Link>
                                    <div className={'documentsCount-overlay'}>
                                        <div className={'documentsCount-row'}>
                                            <img src={file} className={'documentsCount-img__overlay'} alt={'file'}/>
                                            <span className={'documentCount-overlay__text'}>
                                            {category.documentsCount.documentCount} құжат
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))
                        }
                    </div>
                </div>
                <div className="pagination-container">
                    <Pagination
                        count={totalPages}
                        page={page}
                        onChange={handleChange}
                        shape="rounded"
                        siblingCount={1}
                        color="primary"
                    />
                </div>
            </div>
          {categories && categories.length >= 1 ? <Footer/> : ''}
    </div>
    )
};

export default HomePage;
