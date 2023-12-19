import React, { useEffect } from 'react';
import { useSelector, useDispatch } from "react-redux";
import './LegalGroundsPage.scss';
import {fetchLegalGrounds} from "../../store/slices/legalGroundsSlice.js";
import file from '../../assets/file-04.svg'
import linkArrow from '../../assets/img/link-arrow.svg'
import Footer from '../../components/Footer/Footer.jsx';

const LegalGroundsPage = () => {
    const { legalGrounds } = useSelector(state => state.legalGrounds)
    const dispatch = useDispatch()
    
    useEffect(() => {
        dispatch(fetchLegalGrounds())
    }, [dispatch])
    
    console.log(legalGrounds)
    
    return (
        <div className={'page'}>
        <div className={'page__wrapper'}>
            <div className={'page__header'}>
                Құқықтық негіздер
            </div>
            <div className={'links__wrapper'}>
                {
                    legalGrounds.map(el => (
                        <div key={el.uri} className={'legal__ground-container'}>
                            <div className={'legal__ground-header'}>
                                <img src={file} alt={'file'} className={'file-image'} />
                                <a href={el.uri} target={'_blank'}>
                                    <img src={linkArrow} alt={'link'}/>
                                </a>
                            </div>
                            <div>
                                {el.nameKz}
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

export default LegalGroundsPage;