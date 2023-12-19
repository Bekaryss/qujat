import React from 'react';
import SwaggerUI from 'swagger-ui-react'
import 'swagger-ui-react/swagger-ui.css'
import Footer from '../../components/Footer/Footer';
const OpenApiPage = () => {
    return (
        <div>
            <SwaggerUI url="https://qujat-temp-front-api.zonakomforta.kz/swagger/openapi/swagger.json" />
            <Footer/>
         </div>
    );
};

export default OpenApiPage;