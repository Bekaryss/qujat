import React, {useState, useEffect} from 'react';
import { useSelector, useDispatch } from "react-redux";
import {fetchSubCategories, setCurrentSubCategory} from "../../store/slices/subCategoriesSlice.js";
import { Link, useParams } from 'react-router-dom'
import './SubCategoriesPage.scss';
import chevronLeft from '../../assets/img/chevron-left.svg'
import folder from "../../assets/folder.svg";
import file from "../../assets/file-04.svg";
import Footer from '../../components/Footer/Footer.jsx';


const SubCategoriesPage = () => {
    const { categoryId } = useParams()
    const { currentCategory } = useSelector(state => state.categories)
    const { subCategories } = useSelector(state => state.subCategories)
    const dispatch = useDispatch()

    useEffect(() => {
        dispatch(fetchSubCategories(categoryId))
    }, [dispatch]);
    
    return (
        <div className={'page'}>
            <div className={'page__wrapper'}>
                <Link to={'/'} className={'breadcrumbs__link'}>
                    <img src={chevronLeft} alt={'chevron left'}/>
                    <span className={'breadcrumbs__link-text'}>Артқа</span>
                </Link>
                <div className={'page__header'}>
                    {currentCategory.nameKz}
                </div>
                <div className={'subCategories'}>
                    {
                        subCategories.map(subCategory => (
                            <div key={subCategory.id} className={'category-item'}>
                                <img src={folder} alt={'folder'}/>
                                <Link to={`/${currentCategory.id}/subCategories/${subCategory.id}/documents`}
                                      onClick={() => dispatch(setCurrentSubCategory(subCategory))}
                                      className={'text-overlay'}>
                                      {subCategory.nameKz}
                                </Link>
                                <div className={'documentsCount-overlay'}>
                                    <div className={'documentsCount-row'}>
                                        <img src={file} className={'documentsCount-img__overlay'} alt={'file'}/>
                                        <span className={'documentCount-overlay__text'}>
                                            {subCategory.documentsCount.documentCount} құжат
                                        </span>
                                    </div>
                                </div>
                            </div>
                        ))
                    }
                </div>
            </div>
            <Footer/>
        </div>
    );
};

export default SubCategoriesPage;