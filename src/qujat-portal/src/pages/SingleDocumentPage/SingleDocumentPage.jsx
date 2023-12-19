import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from "react-redux";
import './SingleDocumentPage.scss'
import chevronLeft from "../../assets/img/chevron-left.svg";
import { Link, useParams } from "react-router-dom";
import DocViewer, { DocViewerRenderers } from "@cyntler/react-doc-viewer";
import {Button} from "@mui/material";
import share from '../../assets/img/share.svg'
import download from '../../assets/img/download.svg'
import print from '../../assets/img/print.svg'
import instance from '../../axios/axios.js'
import Footer from '../../components/Footer/Footer.jsx'
import {fetchSingleDocument} from '../../store/slices/singleDocumentSlice.js'
import {setCurrentCategory} from '../../store/slices/categoriesSlice.js'
import {setCurrentSubCategory} from '../../store/slices/subCategoriesSlice.js'

const SingleDocumentPage = () => {
    const { categoryId, subCategoryId, documentId } = useParams()
    const { singleDocument } = useSelector(state => state.singleDocument)
    
    const [docs, setDocs] = useState([])
    const [email, setEmail] = useState('')
    
    const dispatch = useDispatch()
    
    const customButtonStyle = {
        backgroundColor: '#E7F6FF',
        width: '75%'
    };

    const customSendButtonStyle = {
        backgroundColor: '#0193EB',
        color: '#FFFFFF',
        width: '75%'
    }

    const handlePdfView = () => {
        window.open(currentDocument.sourceContentBlob.uri, '_blank');
    };
    
    const handleEmailSend = async () => {
        const response = await instance.post(`/api/1/frontend/single-document-page/${currentDocument.id}/actions/send-to`, {
            email
        })
        console.log(response)
    }
    
    const sendPrintedDocumentStatistic = async () => {
        const response = await instance.post(`/api/1/frontend/statistics-triggers/category/${currentCategory.id}/subcategories/${currentSubCategory.id}/document/${currentDocument.id}/downloaded`)
        
        console.log(response)
    }

    const handlePrint = async () => {
        try {
            // Fetch the document content
            const response = await fetch(currentDocument.sourceContentBlob.uri);
            const blob = await response.blob();

            // Create a Blob URL
            const blobUrl = URL.createObjectURL(blob);

            // Create an iframe to load the Blob URL
            const iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            document.body.appendChild(iframe);

            // Set the iframe source to the Blob URL
            iframe.src = blobUrl;

            // Wait for the iframe to load
            iframe.onload = () => {
                // Trigger the print dialog
                iframe.contentWindow.print();

                // Clean up: remove the iframe and revoke the Blob URL
                document.body.removeChild(iframe);
                URL.revokeObjectURL(blobUrl);
            };
            
            sendPrintedDocumentStatistic()
        } catch (error) {
            console.error('Error fetching document:', error);
        }
    };
    
    const sendViewedDocumentStatistic = async () => {
        const response = await instance.post(`/api/1/frontend/statistics-triggers/category/${categoryId}/subcategories/${subCategoryId}/document/${singleDocument.id}/viewed`)
        
        console.log(response)
    }
    
    const sendDownloadedDocumentStatistic = async () => {
        const response = await instance.post(`/api/1/frontend/statistics-triggers/category/${categoryId}/subcategories/${subCategoryId}/document/${singleDocument.id}/downloaded`)
        
        console.log(response)
    }

    useEffect(() => {
        sendViewedDocumentStatistic()
    }, []);

    useEffect(() => {
        if (singleDocument && singleDocument.sourceContentBlob && singleDocument.sourceContentBlob.uri) {
            setDocs([{
                uri: singleDocument.sourceContentBlob.uri
            }])
        }
    }, [singleDocument]);
    
    console.log(singleDocument)
    
    useEffect(() => {
        dispatch(fetchSingleDocument({
            categoryId,
            subCategoryId,
            documentId,
        }))
    }, [dispatch])
    
    useEffect(() => {
        dispatch(setCurrentCategory(singleDocument?.parentSubcategory?.parentCategory))
        dispatch(setCurrentSubCategory(singleDocument?.parentSubcategory))
    }, [singleDocument])
    
    return (
        <div className={'page'}>
            <div className={'page__wrapper'}>
                <Link to={`/${categoryId}/subCategories/${subCategoryId}/documents`} className={'breadcrumbs__link'}>
                    <img src={chevronLeft} alt={'chevron left'}/>
                    <span className={'breadcrumbs__link-text'}>Артқа</span>
                </Link>
                <div className={'page__header'}>
                    {singleDocument.nameKz}
                </div>
                <div className={'document__wrapper'}>
                    <div className={'single-document'}>
                        <DocViewer documents={docs} pluginRenderers={DocViewerRenderers} />
                    </div>
                    <div className={'document__right-bar'}>
                        <div className={'document__action-container'}>
                            <Button style={customButtonStyle} onClick={handlePdfView}>
                                <img src={share} alt={'share'} />
                                <span>Толтырылған үлгіні көру</span>
                            </Button>
                            <Button style={customButtonStyle}>
                                <img src={download} alt={'download'}/>
                                <a href={singleDocument?.sourceContentBlob?.uri} 
                                   download={singleDocument?.sourceContentBlob?.name}
                                   className={'download__ancor'}
                                   onClick={() => sendDownloadedDocumentStatistic}
                                >
                                    Жүктеу
                                </a>
                            </Button>
                            <Button style={customButtonStyle} onClick={handlePrint}>
                                <img src={print} alt={'print'} />
                                <span>Басып шығару</span>
                            </Button>
                        </div>
                        <div className={'send-email__container'}>
                            <input
                                type="text"
                                placeholder="Электрондық поштаны еңгізіңіз"
                                className={'send-email__input'}
                                value={email}
                                onChange={e => setEmail(e.target.value)}
                            />
                            <Button style={customSendButtonStyle} onClick={handleEmailSend}>
                                Жіберу
                            </Button>
                        </div>
                    </div>
                </div>
            </div>
            <Footer/>
        </div>
    );
};

export default SingleDocumentPage
