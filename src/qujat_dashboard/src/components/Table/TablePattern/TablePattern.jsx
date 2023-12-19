import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link, useLocation, useNavigate } from "react-router-dom";
import {
  TableContainer,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Button,
  Paper,
  Box,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  IconButton,
  styled,
} from "@mui/material";

import AddPattern from "../../../components/Modals/Pattern/AddPattern";
import TablePagination from "../../Pagination/TablePagination";
import DeletePattern from "../../Modals/Pattern/DeletePattern";

import { MdEdit } from "react-icons/md";
import { MdOutlineDeleteSweep } from "react-icons/md";

import { IoIosArrowUp, IoIosArrowDown } from "react-icons/io";

import {
  fetchDocuments,
  changePageIndex,
  setDocumentId,
} from "../../../redux/slices/documentsSlice";

const AddButton = styled(Button)({
  backgroundColor: "var(--main-color-btn)",
  color: "#fff",
  padding: 11,
  "&:hover": {
    backgroundColor: "#0486d1",
  },
  "@media (max-width: 475px)": {
    width: "100%",
    marginTop: 8,
  },
});

const TablePattern = () => {
  const dispatch = useDispatch();
  const location = useLocation();
  const navigate = useNavigate();

  const [isOpenPattern, setIsOpenPattern] = useState(false);
  const [isDeletePattern, setIsDeletePattern] = useState(false);
  const [dataLength, setDataLength] = useState();
  const [search, setSearch] = useState("");
  const [sortProperty, setsortProperty] = useState(0);
  const [sortOrder, setSortOrder] = useState(0);
  const [selectedRow, setSelectedRow] = useState([false, false, false, false]);

  const { documentId, documents, pageIndex } = useSelector(
    (state) => state.documents
  );
  // const isLoadingActs = isLoading === true;

  const pageSize = documents?.pageSize; // Ваш размер страницы
  const totalSize = documents?.totalSize; // Общее количество элементов (totalSize)

  const handleChangePage = (_, page) => {
    if (page === 0) {
      page += 1;
    }
    dispatch(changePageIndex(page - 1));
    dispatch(fetchDocuments(page - 1));
    navigate(`${location.pathname}?page=${page}`);
  };

  //HADNLER OPEN CREATE PATTERN
  const handleOpenCreate = () => {
    setIsOpenPattern(true);
  };
  const handleCloseCreate = () => {
    setIsOpenPattern(false);
  };

  //HANDLER DELETE PATTERN
  const handleOpenDelete = (id) => {
    dispatch(setDocumentId(id));
    setIsDeletePattern(true);
  };

  const handleCloseDelete = () => {
    setIsDeletePattern(false);
  };

  const handleOnUpdateDoc = (id) => {
    dispatch(setDocumentId(id));
  };

  const onChangeSearch = (event) => {
    setSearch(event.target.value);
  };

  const onChangeSortOrder = (event) => {
    console.log(event.target.value);
    setSortOrder(event.target.value);
  };

  const sort = (id) => {
    selectedRow[id - 1] = !selectedRow[id - 1];
    setsortProperty(id - 1);
    if (selectedRow[id - 1] == true) {
      setSortOrder(0);
    } else {
      setSortOrder(1);
    }
  };

  useEffect(() => {
    dispatch(
      fetchDocuments({
        pageIndex: pageIndex,
        search: search,
        sortProperty: sortProperty,
        sortOrder: sortOrder,
      })
    );
  }, [dispatch, dataLength, documentId, search, sortOrder, sortProperty]);

  const tableHead = [
    {
      id: 1,
      name: "Құжат үлгісінің қазақша аты",
    },
    {
      id: 2,
      name: "Құжат үлгісінің орысша аты",
    },
    {
      id: 3,
      name: "Құрылған күні",
    },
    {
      id: 4,
      name: "Соңғы жаңартылған күні",
    },
  ];

  return (
    <div className="table">
      <div className="table_head">
        <Box
          sx={{
            display: "flex",
            width: "100%",
            justifyContent: "space-between  ",
          }}
        >
          <Box sx={{ width: "80%", display: "flex", alignItems: "center" }}>
            <TextField
              sx={{ width: "100%" }}
              type="text"
              size="small"
              placeholder="Құжаттың атын еңгізніз"
              onChange={onChangeSearch}
            />
            <FormControl size="small" fullWidth sx={{ marginLeft: 1 }}>
              <InputLabel id="filter_one">Сұрыптау</InputLabel>
              <Select
                labelId="filter_one"
                id="fileter_one"
                label="Фильтр"
                size="small"
                value={sortProperty}
              >
                <MenuItem value={0}>Құжаттың қазакша аты бойынша</MenuItem>
                <MenuItem value={1}>Құжаттың орысша аты бойынша</MenuItem>
                <MenuItem value={2}>Құрылған күні бойынша</MenuItem>
                <MenuItem value={3}>Өзгертілген күні бойынша</MenuItem>
              </Select>
            </FormControl>
            <FormControl size="small" fullWidth sx={{ marginLeft: 1 }}>
              <InputLabel id="filter_one">Сұрыптау</InputLabel>
              <Select
                labelId="filter_one"
                id="fileter_one"
                label="Фильтр"
                size="small"
                value={sortOrder}
              >
                <MenuItem value={0}>Өсу бойынша сұрыптау</MenuItem>
                <MenuItem value={1}>Төмендеу бойынша сұрыптау</MenuItem>
              </Select>
            </FormControl>
          </Box>
          <Link to="/pattern/new-document">
            <AddButton>
              <span className="btn_text">Қосу</span>
            </AddButton>
          </Link>
        </Box>
      </div>
      <TableContainer
        component={Paper}
        sx={{ height: `${10 * 72}px`, overflowY: "hidden", boxShadow: "none" }}
      >
        <Table aria-label="simple table">
          <TableHead className="table_content_head">
            <TableRow sx={{ alignItems: "center" }}>
              <TableCell>
                <h2>#</h2>
              </TableCell>
              {tableHead.map((item) => (
                <TableCell
                  key={item.id}
                  sx={{
                    cursor: "pointer",
                  }}
                  onClick={() => sort(item.id)}
                >
                  <IconButton aria-label="expand row" size="small">
                    {selectedRow[item.id - 1] === false ? (
                      <IoIosArrowUp className="arrow_icon_size" />
                    ) : (
                      <IoIosArrowDown className="arrow_icon_size" />
                    )}
                  </IconButton>
                  <h2>{item.name}</h2>
                </TableCell>
              ))}
              <TableCell>
                <h2>Әрекет</h2>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody colSpan="">
            {documents.responseData?.map((item) => (
              <TableRow
                key={item.id}
                sx={{
                  cursor: "pointer",
                }}
                className="table_item"
              >
                <TableCell>{item.id}</TableCell>
                <TableCell sx={{ wordWrap: true, width: "28%" }}>
                  {item.nameKz}
                </TableCell>
                <TableCell sx={{ wordWrap: true, width: "28%" }}>
                  {item.nameRu}
                </TableCell>
                <TableCell>
                  {new Date(item.createdOn).toLocaleString()}
                </TableCell>
                <TableCell>
                  {new Date(item.lastUpdatedOn).toLocaleString()}
                </TableCell>
                <TableCell>
                  <Box className="table_action">
                    <Link
                      to={`/edit-document/${item.id}`}
                      style={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "1px solid var(--black)",
                        borderRadius: "8px",
                        padding: "4px",
                        cursor: "pointer",
                        marginRight: "16px",
                        transition: "var(--tran-bg-03)",
                        "&:hover": {
                          background: "var(--main-color-btn)",
                        },
                      }}
                      onClick={() => {
                        handleOnUpdateDoc(item.id);
                      }}
                    >
                      <MdEdit style={{ fontSize: "20px" }} />
                    </Link>
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "1px solid var(--black)",
                        borderRadius: "8px",
                        padding: "4px",
                        cursor: "pointer",
                        marginRight: "16px",
                        transition: "var(--tran-bg-03)",
                        "&:hover": {
                          background: "var(--main-color-btn)",
                        },
                      }}
                      onClick={() => handleOpenDelete(item.id)}
                    >
                      <MdOutlineDeleteSweep
                        style={{
                          fontSize: "20px",
                          color: "var(--main-red)",
                        }}
                      />
                    </Box>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Box
        spacing={2}
        sx={{
          width: "100%",
          display: "flex",
          alignContent: "center",
          justifyContent: "center",
          padding: 3,
        }}
      >
        <TablePagination
          totalSize={totalSize}
          pageIndex={pageIndex}
          pageSize={pageSize}
          handleChangePage={handleChangePage}
          renderItem={(item) => (
            <PaginationItem
              component={StyledLink}
              to={`${location.pathname}?page=${item.page}`}
              {...item}
            />
          )}
        />
      </Box>
      <AddPattern
        isOpenPattern={isOpenPattern}
        handleCloseCreate={handleCloseCreate}
      />
      <DeletePattern
        deleteItem={isDeletePattern}
        handleCloseDeleteModal={handleCloseDelete}
        id={documentId}
      />
    </div>
  );
};

export default TablePattern;
