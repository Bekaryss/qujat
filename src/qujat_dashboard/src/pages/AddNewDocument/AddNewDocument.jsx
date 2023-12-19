import React, { useEffect, useState } from "react";
import {
  Box,
  Divider,
  TextField,
  FormControl,
  MenuItem,
  InputLabel,
  Select,
  Button,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { v4 as uuidv4 } from "uuid";

import { fetchCategory } from "../../redux/slices/categorySlice";
import { fetchSubCategories } from "../../redux/slices/subCategoriesSlice";

import instance from "../../axios/axios";

import MultiplySelectSubCategory from "../../components/MultiplySelectSubCategory/MultiplySelectSubCategory";
import UpLoadFile from "./UpLoadFile/UpLoadFile";
import UpLoadFilePDF from "./UpLoadFile/UpLoadFilePDF";

import "./AddNewDocument.css";

const AddNewDocument = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { categories, isLoading } = useSelector((state) => state.categories);
  const isLoadingCategories = isLoading === true;
  const { subCategories } = useSelector((state) => state.subCategories);

  const [selectedCategory, setSelectedCategory] = useState("");
  const [selectedCategoryList, setSelectedCategoryList] = useState([]);
  const [selectedValues, setSelectedValues] = useState([]);
  const [uploadedFileId, setUploadedFileId] = useState(null);
  const [uploadedPdf, setUploadPdf] = useState(null);
  const [showSelectedCategoryList, setShowSelectedCategoryList] = useState([]);
  const newSelectId = uuidv4();
  const [additionalSelects, setAdditionalSelects] = useState([newSelectId]);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm({
    defaultValues: {
      nameKz: "",
      nameRu: "",
      parentSubcategoryIds: [],
      sourceContentBlobId: 0,
      filledContentBlobId: null,
    },
    mode: "onBlur",
  });

  const handleAddCategory = () => {
    const newSelectId = uuidv4();
    setAdditionalSelects((prevSelects) => [...prevSelects, newSelectId]);
    console.log(additionalSelects);
  };

  const handleChangeSelectSubCategory = (value, selectId) => {
    setSelectedValues((prev) => ({ ...prev, [selectId]: value }));

    const id = uuidv4();
    const selectedCategoryObj = {
      id: id,
      name: selectedCategory,
      subCategories: value.map((item) => item),
    };

    const existingIndex = selectedCategoryList.findIndex(
      (obj) => obj.name === selectedCategory
    );

    if (existingIndex !== -1) {
      // Object with the same id already exists, update subCategories
      const updatedObject = {
        ...selectedCategoryList[existingIndex],
        subCategories: [
          // ...selectedCategoryList[existingIndex].subCategories,
          ...selectedCategoryObj.subCategories,
        ],
      };

      // Update the object in the array
      selectedCategoryList[existingIndex] = updatedObject;
    } else {
      // Object with the same id does not exist, add a new object
      selectedCategoryList.push(selectedCategoryObj);
    }

    setShowSelectedCategoryList(selectedCategoryList);

    console.log(showSelectedCategoryList);
  };

  const handleCategoryChange = (event, selectId) => {
    const value = event.target.value;
    setSelectedCategory((prev) => ({ ...prev, [selectId]: value }));
  };

  const getSubCategory = (id) => {
    dispatch(fetchSubCategories(id));
  };

  const handleFileUpload = (id) => {
    console.log(id);
    // Обновляем contentBlobIds после успешной загрузки файла
    setUploadedFileId(id);
  };

  const handleUploadPDF = (id) => {
    setUploadPdf(id);
  };

  // const setCategorySubCategory = (data) => {
  //   setSelectedValues(data);
  // };

  const onSubmit = async (data) => {
    const { fileType, category, ...formData } = data;
    formData.sourceContentBlobId = uploadedFileId; // Обновляем contentBlobIds
    formData.filledContentBlobId = uploadedPdf;
    console.log(formData);
    try {
      const response = await instance.post("/api/1/documents", formData, {
        headers: { accept: "*/*" },
      });

      if (response.status === 200) {
        setValue("nameKz", response.data.responseData.nameKz);
        navigate("/pattern");
      }
    } catch (error) {
      console.error("Произошла ошибка:", error);
    }
  };

  useEffect(() => {
    console.log(uploadedFileId);
    // if (!isLoadingCategories && uploadedFileId !== null) {
    dispatch(fetchCategory());
    // }

    // console.log(selectedCategoryList);
  }, [dispatch, selectedCategoryList]);

  useEffect(() => {
    // Update the default value for parentSubcategoryIds
    const parentSubcategoryIds = Object.values(selectedValues)
      .flat() // Flatten the arrays
      .map((item) => item.id);

    setValue("parentSubcategoryIds", parentSubcategoryIds);
  }, [selectedValues, showSelectedCategoryList]);

  return (
    <div className="add_new_document_container">
      <h1>Жаңа құжат</h1>
      <Box className="new_document_wrapper">
        <p>Құжатты жасау үшін деректерді енгізіңіз</p>
        <Divider
          sx={{
            display: "flex",
            width: "100%",
            height: "2px",
            background: "var(--gray-800)",
          }}
        />
        <Box sx={{ width: "80%", marginTop: 3 }}>
          <h3>Алдымен файлды жүктеп алыңыз</h3>
          <UpLoadFile onFileUpload={handleFileUpload} />
        </Box>
        <Box sx={{ width: "80%", marginTop: 2 }}>
          <h3>PDF файл</h3>
          <UpLoadFilePDF handleUploadPDF={handleUploadPDF} />
        </Box>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Box sx={{ width: "100%", marginTop: 3 }}>
            <span className="main_text">
              Құжат үлгісінің қазақша атын енгізіңіз
            </span>
            <TextField
              size="medium"
              sx={{ marginTop: 1 }}
              fullWidth
              label="Құжат үлгісінің атын енгізіңіз"
              error={Boolean(errors.nameKz?.message)}
              {...register("nameKz", {
                required: "Құжат үлгісінің атын енгізіңіз",
              })}
              disabled={uploadedFileId === null}
            />
          </Box>
          <Box sx={{ width: "100%", marginTop: 3 }}>
            <span className="main_text">
              Құжат үлгісінің орысша атын енгізіңіз
            </span>
            <TextField
              size="medium"
              sx={{ marginTop: 2 }}
              fullWidth
              label="Құжат үлгісінің орысша атын енгізіңіз"
              error={Boolean(errors.nameRu?.message)}
              {...register("nameRu", {
                required: "Құжат үлгісінің орысша атын енгізіңіз",
              })}
              disabled={uploadedFileId === null}
            />
          </Box>

          <Box sx={{ display: "flex", width: "100%", flexDirection: "column" }}>
            <Box
              sx={{ display: "flex", width: "100%", flexDirection: "column" }}
            >
              {additionalSelects.map((selectId) => (
                <Box
                  key={selectId}
                  sx={{
                    display: "flex",
                    width: "100%",
                    marginTop: 2,
                    gap: "30px",
                  }}
                >
                  <Box
                    sx={{
                      width: "50%",
                      marginTop: 2,
                      display: "flex",
                      flexDirection: "column",
                    }}
                  >
                    <span className="main_text">
                      Құжат категориясын таңдаңыз
                    </span>
                    <FormControl sx={{ marginTop: 2 }} size="small">
                      <InputLabel id="category">
                        Құжат категориясын таңдаңыз
                      </InputLabel>
                      <Select
                        labelId="category"
                        label="Құжат категориясының таңдаңыз"
                        value={selectedCategory[selectId] || ""}
                        onChange={(event) => {
                          handleCategoryChange(event, selectId);
                          setValue("category", event.target.value);
                        }}
                        sx={{ width: "100%" }}
                        // disabled={uploadedFileId === null}
                        size="small"
                      >
                        {!isLoadingCategories &&
                          categories.map((item) => (
                            <MenuItem
                              key={item.id}
                              value={item.nameKz} // Make sure this is the correct value
                              onClick={() => getSubCategory(item.id)}
                            >
                              {item.nameKz}
                            </MenuItem>
                          ))}
                      </Select>
                    </FormControl>
                  </Box>
                  <Box sx={{ width: "50%", marginTop: 2 }}>
                    <span className="main_text">
                      Құжатның ішкі категориясын таңдаңыз
                    </span>
                    <MultiplySelectSubCategory
                      subCategories={subCategories}
                      selectedSubCategories={(value) =>
                        handleChangeSelectSubCategory(value, selectId)
                      }
                      selectedValues={selectedValues[selectId] || []}
                      selectedCategory={selectedCategory[selectId] || ""}
                    />
                  </Box>
                </Box>
              ))}
              <Button
                variant="outlined"
                color="success"
                sx={{ marginTop: 2, width: "30%" }}
                onClick={handleAddCategory}
              >
                Добавить категорию
              </Button>
            </Box>
          </Box>
          <Divider
            sx={{
              display: "flex",
              width: "100%",
              height: "2px",
              background: "var(--gray-800)",
              marginTop: 3,
              marginBottom: 3,
            }}
          />
          <Box sx={{ marginTop: 3 }}>
            <Button
              variant="contained"
              type="submit"
              disabled={uploadedFileId === null}
            >
              Қосу
            </Button>
          </Box>
        </form>
      </Box>
    </div>
  );
};

export default AddNewDocument;
