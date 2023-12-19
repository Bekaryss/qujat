import React, { useEffect, useState } from 'react';
import { useSelector, useDispatch } from "react-redux";
import './DocumentsPage.scss';
import chevronLeft from "../../assets/img/chevron-left.svg";
import { Link, useParams} from "react-router-dom";
import {fetchDocuments, fetchTopDocuments, setCurrentDocument} from "../../store/slices/documentsSlice.js";
import more from '../../assets/img/more.svg'
import { Button, Menu, MenuItem } from '@mui/material';
import download from '../../assets/img/download.svg'
import share from "../../assets/img/share.svg";
import print from "../../assets/img/print.svg";
import Footer from '../../components/Footer/Footer.jsx';

const DocumentsPage = () => {
    const { categoryId, subCategoryId } = useParams()
    const { currentCategory } = useSelector(state => state.categories)
    const { currentSubCategory } = useSelector(state => state.subCategories)
    const { documents, topDocuments } = useSelector(state => state.documents)
    const dispatch = useDispatch()

    const [anchorEl, setAnchorEl] = useState(null);
    const [selectedDocument, setSelectedDocument] = useState(null);

    const handleMoreClick = (event, document) => {
        setAnchorEl(event.currentTarget);
        setSelectedDocument(document);
    };

    const handleCloseMenu = () => {
        setAnchorEl(null);
    };

    const handlePdfView = () => {
        window.open(currentDocument.sourceContentBlob.uri, '_blank');
    };
    
    useEffect(() => {
        dispatch(fetchDocuments({
            categoryId,
            subCategoryId
        }))
        dispatch(fetchTopDocuments())
    }, [dispatch])
    
    return (
        <div className={'page'}>
            <div className={'page__wrapper'}>
                <Link to={`/${categoryId}/subCategories`} className={'breadcrumbs__link'}>
                    <img src={chevronLeft} alt={'chevron left'}/>
                    <span className={'breadcrumbs__link-text'}>Артқа</span>
                </Link>
                <div className={'page__header'}>
                    {currentSubCategory.nameKz}
                </div>
                <div className={'documents-wrapper'}>
                    <div className={'documents'}>
                        {
                            documents.map(document => (
                                <div key={document.id} className={'document-container'}>
                                    <Link
                                        to={`/${currentCategory.id}/subCategories/${currentSubCategory.id}/documents/${document.id}`}
                                        onClick={() => dispatch(setCurrentDocument(document))}
                                        className={'document-link'}
                                    >
                                        {document.nameKz}
                                    </Link>
                                    {/*<img src={more} alt={'more'} className={'more-button'} />*/}
                                    <img
                                        src={more}
                                        alt={'more'}
                                        className={'more-button'}
                                        onClick={(event) => handleMoreClick(event, document)}
                                    />
                                    <Menu
                                        anchorEl={anchorEl}
                                        open={Boolean(anchorEl)}
                                        onClose={handleCloseMenu}
                                        anchorOrigin={{
                                            vertical: 'bottom',
                                            horizontal: 'left',
                                        }}
                                        transformOrigin={{
                                            vertical: 'top',
                                            horizontal: 'left',
                                        }}
                                        >
                                        <MenuItem onClick={handleCloseMenu}>
                                            <Button onClick={handlePdfView}>
                                                <img src={share} alt={'share'} />
                                                <span>Алдын ала қарау</span>
                                            </Button>
                                        </MenuItem>
                                        <MenuItem onClick={handleCloseMenu}>
                                            <Button>
                                                <img src={download} alt={'download'}/>
                                                <a
                                                    className={'download__ancor'}
                                                >
                                                    Жүктеу
                                                </a>
                                            </Button>
                                        </MenuItem>
                                        <MenuItem onClick={handleCloseMenu}>
                                            <Button>
                                                <img src={print} alt={'print'} />
                                                <span>Басып шығару</span>
                                            </Button>
                                        </MenuItem>
                                    </Menu>
                                </div>
                            ))
                        }
                    </div>
                    <div className={'top-documents__container'}>
                        <div className={'top-documents__header'}>
                            Көп жүктелген қүжаттар
                        </div>
                        <div className={'top-documents__body'}>
                            {
                                topDocuments.map(topDocument => (
                                    <Link key={topDocument.id} 
                                          to={`/${currentCategory.id}/subCategories/${currentSubCategory.id}/documents/${topDocument.id}`}
                                          className={'top-document__container'}
                                          onClick={() => dispatch(setCurrentDocument(topDocument))}
                                    >
                                        <div>
                                            {topDocument.nameKz}
                                        </div>
                                    </Link>
                                ))
                            }
                        </div>
                    </div>
                </div>
            </div>
            <Footer/>
        </div>
    );
};

export default DocumentsPage;