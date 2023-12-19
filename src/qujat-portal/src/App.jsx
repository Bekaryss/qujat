import './App.css'
import Navbar from "./components/Navbar/Navbar";
import { Routes, Route} from "react-router-dom";
import HomePage from "./pages/HomePage/HomePage.jsx";
import LegalGroundsPage from "./pages/LegalGroundsPage/LegalGroundsPage.jsx";
import OpenApiPage from "./pages/OpenApiPage/OpenApiPage.jsx";
import SubCategoriesPage from "./pages/SubCategoriesPage/SubCategoriesPage.jsx";
import DocumentsPage from "./pages/DocumentsPage/DocumentsPage.jsx";
import SingleDocumentPage from "./pages/SingleDocumentPage/SingleDocumentPage.jsx";
import SearchPage from './pages/SearchPage/SearchPage.jsx'

function App() {
  return (
    <div className={'app'}>
        <Navbar />
          <Routes>
            <Route path='/' element={<HomePage />} />
            <Route path={'/search-page/:searchQuery'} element={<SearchPage />} />
            <Route path='/:categoryId/subCategories' element={<SubCategoriesPage />} />
            <Route path={'/:categoryId/subCategories/:subCategoryId/documents'} element={<DocumentsPage />} />
            <Route path={'/:categoryId/subCategories/:subCategoryId/documents/:documentId'} element={<SingleDocumentPage />} />
            <Route path='/legal-grounds' element={<LegalGroundsPage />} />
            <Route path='/open-api' element={<OpenApiPage />} />
          </Routes>
      </div>
  )
}

export default App
